param defaultResourceName string
param location string
param containerVersion string
param developersGroup string
param integrationEnvironment object

param acrLoginServer string
param acrUsername string
@secure()
param acrPassword string

param containerPort int = 8080
param containerAppName string = 'wam-proxy-api'

resource containerAppEnvironments 'Microsoft.App/managedEnvironments@2022-03-01' existing = {
  name: integrationEnvironment.containerAppsEnvironment
  scope: resourceGroup(integrationEnvironment.resourceGroup)
}
resource appConfiguration 'Microsoft.AppConfiguration/configurationStores@2022-05-01' existing = {
  name: integrationEnvironment.appConfiguration
  scope: resourceGroup(integrationEnvironment.resourceGroup)
}

module serviceNameConfigurationValue 'configuration-value.bicep' = {
  name: 'serviceNameConfigurationValue'
  scope: resourceGroup(integrationEnvironment.resourceGroup)
  params: {
    appConfigurationName: integrationEnvironment.appConfiguration
    settingName: 'Services:ProxyService'
    settingValue: apiContainerApp.name
  }
}

resource apiContainerApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: '${defaultResourceName}-aca'
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    managedEnvironmentId: containerAppEnvironments.id

    configuration: {
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: containerPort
        transport: 'auto'
        allowInsecure: false
        traffic: [
          {
            weight: 100
            latestRevision: true
          }
        ]
      }
      dapr: {
        enabled: true
        appPort: containerPort
        appId: containerAppName
      }
      secrets: [
        {
          name: 'container-registry-password'
          value: acrPassword
        }
      ]
      registries: [
        {
          server: acrLoginServer
          username: acrUsername
          passwordSecretRef: 'container-registry-password'
        }
      ]
    }

    template: {
      containers: [
        {
          image: '${acrLoginServer}/${containerAppName}:${containerVersion}'
          name: containerAppName
          resources: {
            cpu: json('0.25')
            memory: '0.5Gi'
          }
          env: [
            {
              name: 'AzureAppConfiguration'
              value: appConfiguration.properties.endpoint
            }
          ]

        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 6
        rules: [
          {
            name: 'http-rule'
            http: {
              metadata: {
                concurrentRequests: '30'
              }
            }
          }
        ]
      }
    }
  }
}

module roleAssignmentsModule 'all-role-assignments.bicep' = {
  name: 'roleAssignmentsModule'
  params: {
    containerAppPrincipalId: apiContainerApp.identity.principalId
    developersGroup: developersGroup
    integrationResourceGroupName: integrationEnvironment.resourceGroup
  }
}
