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
        // 設定 API 路由：當發出 GET 請求至 /api/video/generateSasToken 時，會調用這個方法
        [HttpGet("generateSasToken")]
        public IActionResult GenerateSasToken(string blobName)
        {
            // 建立一個 BlobServiceClient 物件，用於訪問 Azure Blob Storage，使用儲存體帳戶的連接字串進行認證
            string _blobConnectionString = "";
            var blobServiceClient = new BlobServiceClient(_blobConnectionString);
            string _containerName = "";
            // 取得對特定容器的 BlobContainerClient，這個容器是用來存放 Blob（檔案）
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            // 建立一個 BlobClient，表示將要存取的特定 Blob（檔案）
            var blobClient = blobContainerClient.GetBlobClient(blobName);

            // 建立一個 BlobSasBuilder 物件，用於生成 SAS token 的相關設定
            var sasBuilder = new BlobSasBuilder
            {
                // 設定要存取的容器名稱
                BlobContainerName = _containerName,

                // 設定要存取的 Blob 名稱（即檔案名稱）
                BlobName = blobName,

                // 設定資源類型：b 表示這是一個 Blob（單個檔案）
                Resource = "b",

                // 設定 SAS token 的過期時間，這裡設定為 1 小時後過期
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            };

            // 設定 SAS token 的權限，允許進行寫入操作（這樣客戶端才能上傳檔案）
            sasBuilder.SetPermissions(BlobSasPermissions.Write); // 可改成 Read, Delete 等權限

            // 使用 BlobClient 生成對應的 SAS URI，並取得查詢字串部分 (即生成的 SAS token)
            var sasToken = blobClient.GenerateSasUri(sasBuilder).Query;

            // 將生成的 SAS token 作為 JSON 格式返回給客戶端
            return Ok(new { sasToken });

            //前端
//            function uploadVideoDirectly()
//            {
//                const fileInput = document.getElementById('videoFile');
//                const file = fileInput.files[0];
//                const blobName = file.name;

//                // 1. 向後端請求 SAS token
//                fetch(`/ api / video / generateSasToken ? blobName =${ encodeURIComponent(blobName)}`)
//        .then(response => response.json())
//        .then(data => {
//        const sasToken = data.sasToken;

//        // 2. 直接將影片上傳到 Azure Blob Storage
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
