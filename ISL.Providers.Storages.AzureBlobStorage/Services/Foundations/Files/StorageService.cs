﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ISL.Providers.Storages.AzureBlobStorage.Brokers.Storages.Blobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ISL.Providers.Storages.AzureBlobStorage.Services.Foundations.Storages
{
    internal partial class StorageService : IStorageService
    {
        private readonly IBlobStorageBroker blobStorageBroker;

        internal StorageService(IBlobStorageBroker blobStorageBroker)
        {
            this.blobStorageBroker = blobStorageBroker;
        }

        public ValueTask CreateFileAsync(Stream input, string fileName, string container) =>
            TryCatch(async () =>
            {
                await ValidateStorageArgumentsOnCreateAsync(input, fileName, container);

                BlobClient blobClient =
                    this.blobStorageBroker.blobServiceClient
                        .GetBlobContainerClient(container)
                        .GetBlobClient(fileName);

                await blobClient.UploadAsync(input);
            });

        public ValueTask RetrieveFileAsync(Stream output, string fileName, string container) =>
            TryCatch(async () =>
            {
                await ValidateStorageArgumentsOnRetrieveAsync(output, fileName, container);

                BlobClient blobClient =
                    this.blobStorageBroker.blobServiceClient
                        .GetBlobContainerClient(container)
                        .GetBlobClient(fileName);

                await blobClient.DownloadToAsync(output);
            });

        public ValueTask DeleteFileAsync(string fileName, string container) =>
            TryCatch(async () =>
            {
                await ValidateStorageArgumentsOnDeleteAsync(fileName, container);

                BlobClient blobClient =
                    this.blobStorageBroker.blobServiceClient
                        .GetBlobContainerClient(container)
                        .GetBlobClient(fileName);

                await blobClient.DeleteAsync(DeleteSnapshotsOption.None);
            });

        public ValueTask<List<string>> ListFilesInContainerAsync(string container) =>
            TryCatch(async () =>
            {
                await ValidateStorageArgumentsOnListAsync(container);
                List<string> fileNames = new List<string>();

                BlobContainerClient containerClient =
                    this.blobStorageBroker.blobServiceClient
                        .GetBlobContainerClient(container);

                AsyncPageable<BlobItem> blobItems = containerClient.GetBlobsAsync();

                await foreach (BlobItem blobItem in blobItems)
                {
                    fileNames.Add(blobItem.Name);
                }

                return fileNames;
            });


        public ValueTask<string> GetDownloadLinkAsync(string fileName, string container, DateTimeOffset expiresOn) =>
            throw new NotImplementedException();

        public ValueTask CreateContainerAsync(string container) =>
            throw new NotImplementedException();

        public ValueTask SetContainerACLAsync(string container, string accessType, string permissions) =>
            throw new NotImplementedException();

        public ValueTask CreateAndAssignRoleToContainerAsync(string roleName, string container) =>
            throw new NotImplementedException();

        public ValueTask CreateAndAssignManagedIdentityToRoleAsync(string identity, string roleName) =>
            throw new NotImplementedException();
    }
}
