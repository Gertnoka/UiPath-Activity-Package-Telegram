using System;
using System.Activities;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EN.Telegram.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using UiPath.Shared.Activities.Utilities;

namespace EN.Telegram.Activities
{
    [LocalizedDisplayName(nameof(Resources.SendFile_DisplayName))]
    [LocalizedDescription(nameof(Resources.SendFile_Description))]
    public class SendFile : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.SendFile_CHATID_DisplayName))]
        [LocalizedDescription(nameof(Resources.SendFile_CHATID_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> CHATID { get; set; }

        [LocalizedDisplayName(nameof(Resources.SendFile_FilePath_DisplayName))]
        [LocalizedDescription(nameof(Resources.SendFile_FilePath_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> FilePath { get; set; }

        [LocalizedDisplayName(nameof(Resources.SendFile_STATUSCODE_DisplayName))]
        [LocalizedDescription(nameof(Resources.SendFile_STATUSCODE_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> STATUSCODE { get; set; }

        #endregion


        #region Constructors

        public SendFile()
        {
            Constraints.Add(ActivityConstraints.HasParentType<SendFile, TelegramScope>(string.Format(Resources.ValidationScope_Error, Resources.TelegramScope_DisplayName)));
        }

        HttpClient client = new HttpClient();

        public async Task<string> sendFileMethod(string ApiKey, string ChatId, string filePath)
        {

            String fileName = Path.GetFileName(filePath);
            var fileType = fileName.Split('.').Last();
            var fileStreamContent = new StreamContent(File.OpenRead(filePath));
            fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/" + fileType);

            MultipartFormDataContent multipartFormContent = new MultipartFormDataContent();
            multipartFormContent.Add(new StringContent(ChatId), name: "chat_id");
            multipartFormContent.Add(fileStreamContent, name: "document", fileName: fileName);

            var endpoint = new Uri($"https://api.telegram.org/bot{ApiKey}/sendDocument");

            var response = await client.PostAsync(endpoint, multipartFormContent);
            response.EnsureSuccessStatusCode();
            await response.Content.ReadAsStringAsync();
            return response.StatusCode.ToString();


        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (CHATID == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(CHATID)));
            if (FilePath == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(FilePath)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Object Container: Use objectContainer.Get<T>() to retrieve objects from the scope
            var objectContainer = context.GetFromContext<IObjectContainer>(TelegramScope.ParentContainerPropertyTag);

            // Inputs
            var chatID = CHATID.Get(context);
            var filePath = FilePath.Get(context);

            var property = context.DataContext.GetProperties()[TelegramScope.ParentContainerPropertyTag];
            var Container = property.GetValue(context.DataContext) as IObjectContainer;
            string apiKey = Container.Get<string>();

            // Add execution logic HERE
            SendFile sFile = new SendFile();
            Task<string> task = sFile.sendFileMethod(apiKey, chatID, filePath); ;
            await task;
            string result = task.Result;

            // Outputs
            return (ctx) => {
                STATUSCODE.Set(ctx, result);
            };
        }

        #endregion
    }
}

