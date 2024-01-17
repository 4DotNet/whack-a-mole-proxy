targetScope = 'subscription'

param systemName string

@allowed([
  'dev'
  'tst'
  'prd'
])
param environmentName string
param location string = deployment().location
param locationAbbreviation string
param containerVersion string = '1.0.0'
param developersGroup string
param integrationEnvironment object
param acrLoginServer string
param acrUsername string
@secure()
param acrPassword string

var apiResourceGroupName = toLower('${systemName}-${environmentName}-${locationAbbreviation}')

resource apiResourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: apiResourceGroupName
  location: location
}

module resourcesModule 'resources.bicep' = {
  name: 'ResourceModule'
  scope: apiResourceGroup
  params: {
    defaultResourceName: apiResourceGroupName
    location: location
    containerVersion: containerVersion
    integrationEnvironment: integrationEnvironment
    developersGroup: developersGroup
    acrLoginServer: acrLoginServer
    acrUsername: acrUsername
    acrPassword: acrPassword
  }
}