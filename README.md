# Blob storage with C#
Blob storage example with C# to upload and download files from an API.

## Before start
Go to the **appsettings.json** file and set the following properties:  
*BlobConnectionString:* The blob connection string, you need to get it from azure.  
*BlobContainerName:* The name of container where you want to upload files. If the container does not exists the project will create it on startup.  

## Upload API
Check if the file if exists, if it exists it will delete it, then it will upload the file. To test the upload API use the next endpoint:  
*/api/blob/upload*  

## Download API
Validate if the file exists, if not return a BadRequest. If exists, return the file. To test the download API use the next endpoint:  
*api/blob/download?fileName=dummy.pdf*  
Where "fileName" must include also the file extension.  
