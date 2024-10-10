using FluentAssertions;
using ISL.Providers.Storages.AzureBlobStorage.Models.Foundations.Files.Exceptions;
using System.IO;
using System.Threading.Tasks;

namespace ISL.Providers.Storages.AzureBlobStorage.Tests.Unit.Services.Foundations.Files
{
    public partial class StorageTests
    {
        [Theory]
        [MemberData(nameof(InvalidArgumentsStreamHasLength))]
        public async Task ShouldThrowValidationExceptionOnRetrieveIfArgumentsInvalidAsync(Stream invalidStream, string invalidText)
        {
            // given
            string invalidFileName = invalidText;
            string invalidContainer = invalidText;
            Stream invalidInputStream = invalidStream;

            var invalidArgumentStorageException =
                new InvalidArgumentStorageException(
                    message: "Invalid storage service argument(s), please fix the errors and try again.");

            invalidArgumentStorageException.AddData(
                key: "Output",
                values: "Stream is invalid");

            invalidArgumentStorageException.AddData(
                key: "FileName",
                values: "Text is invalid");

            invalidArgumentStorageException.AddData(
                key: "Container",
                values: "Text is invalid");

            var expectedStorageValidationException =
                new StorageValidationException(
                    message: "Storage validation error occurred, please fix errors and try again.",
                    innerException: invalidArgumentStorageException);

            // when
            ValueTask retrieveFileTask =
                this.storageService.RetrieveFileAsync(invalidInputStream, invalidFileName, invalidContainer);

            StorageValidationException actualFileValidationException =
                await Assert.ThrowsAsync<StorageValidationException>(testCode: retrieveFileTask.AsTask);

            // then
            actualFileValidationException
                .Should().BeEquivalentTo(expectedStorageValidationException);

            this.blobServiceClientMock.VerifyNoOtherCalls();
            this.blobContainerClientMock.VerifyNoOtherCalls();
            this.blobClientMock.VerifyNoOtherCalls();
            this.blobStorageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
