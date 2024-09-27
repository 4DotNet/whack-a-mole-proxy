using './main.bicep'

param systemName = 'wam-proxy-api'
param environmentName = 'prd'
param locationAbbreviation = 'ne'
param developersGroup = '397a477e-c6d9-49ff-8652-b434a7a90a82'
param integrationEnvironment = {
  resourceGroup: 'wam-int-prd-rg'
  containerAppsEnvironment: 'wam-int-prd-env'
  appConfiguration: 'wam-int-prd-appcfg'
}
param acrLoginServer = ''
param acrPassword = ''
param acrUsername = ''
