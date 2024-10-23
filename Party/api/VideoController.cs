using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Party.Data;

namespace Party.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<UserController> _localizer;

        public VideoController(ILogger<UserController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        // �]�w API ���ѡG��o�X GET �ШD�� /api/video/generateSasToken �ɡA�|�եγo�Ӥ�k
        [HttpGet("generateSasToken")]
        public IActionResult GenerateSasToken(string blobName)
        {
            // �إߤ@�� BlobServiceClient ����A�Ω�X�� Azure Blob Storage�A�ϥ��x�s��b�᪺�s���r��i��{��
            string _blobConnectionString = "";
            var blobServiceClient = new BlobServiceClient(_blobConnectionString);
            string _containerName = "";
            // ���o��S�w�e���� BlobContainerClient�A�o�Ӯe���O�ΨӦs�� Blob�]�ɮס^
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            // �إߤ@�� BlobClient�A��ܱN�n�s�����S�w Blob�]�ɮס^
            var blobClient = blobContainerClient.GetBlobClient(blobName);

            // �إߤ@�� BlobSasBuilder ����A�Ω�ͦ� SAS token �������]�w
            var sasBuilder = new BlobSasBuilder
            {
                // �]�w�n�s�����e���W��
                BlobContainerName = _containerName,

                // �]�w�n�s���� Blob �W�١]�Y�ɮצW�١^
                BlobName = blobName,

                // �]�w�귽�����Gb ��ܳo�O�@�� Blob�]����ɮס^
                Resource = "b",

                // �]�w SAS token ���L���ɶ��A�o�̳]�w�� 1 �p�ɫ�L��
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            };

            // �]�w SAS token ���v���A���\�i��g�J�ާ@�]�o�˫Ȥ�ݤ~��W���ɮס^
            sasBuilder.SetPermissions(BlobSasPermissions.Write); // �i�令 Read, Delete ���v��

            // �ϥ� BlobClient �ͦ������� SAS URI�A�è��o�d�ߦr�곡�� (�Y�ͦ��� SAS token)
            var sasToken = blobClient.GenerateSasUri(sasBuilder).Query;

            // �N�ͦ��� SAS token �@�� JSON �榡��^���Ȥ��
            return Ok(new { sasToken });

            //�e��
//            function uploadVideoDirectly()
//            {
//                const fileInput = document.getElementById('videoFile');
//                const file = fileInput.files[0];
//                const blobName = file.name;

//                // 1. �V��ݽШD SAS token
//                fetch(`/ api / video / generateSasToken ? blobName =${ encodeURIComponent(blobName)}`)
//        .then(response => response.json())
//        .then(data => {
//        const sasToken = data.sasToken;

//        // 2. �����N�v���W�Ǩ� Azure Blob Storage
//        const uploadUrl = `https://<your-storage-account>.blob.core.windows.net/<your-container-name>/${blobName}?${sasToken}`;
//        fetch(uploadUrl, {
//        method: 'PUT',
//                headers:
//            {
//                'x-ms-blob-type': 'BlockBlob'
//                },
//                body: file
//    })
//            .then(() => {
//        console.log('Video uploaded successfully.');
//    })
//            .catch(error => {
//                console.error('Error uploading video:', error);
//            });
//        });
//}


        }

    }
}
