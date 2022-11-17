using System;
using System.Activities;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EN.Telegram.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using UiPath.Shared.Activities.Utilities;

namespace EN.Telegram.Activities
{
    [LocalizedDisplayName(nameof(Resources.SendMessage_DisplayName))]
    [LocalizedDescription(nameof(Resources.SendMessage_Description))]
    public class SendMessage : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.SendMessage_CHATID_DisplayName))]
        [LocalizedDescription(nameof(Resources.SendMessage_CHATID_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> CHATID { get; set; }

        [LocalizedDisplayName(nameof(Resources.SendMessage_MESSAGE_DisplayName))]
        [LocalizedDescription(nameof(Resources.SendMessage_MESSAGE_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> MESSAGE { get; set; }

        [LocalizedDisplayName(nameof(Resources.SendMessage_STATUSCODE_DisplayName))]
        [LocalizedDescription(nameof(Resources.SendMessage_STATUSCODE_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> STATUSCODE { get; set; }

        #endregion


        #region Constructors

        public SendMessage()
        {
            Constraints.Add(ActivityConstraints.HasParentType<SendMessage, TelegramScope>(string.Format(Resources.ValidationScope_Error, Resources.TelegramScope_DisplayName)));
        }

        HttpClient client = new();


        public async Task<string> sendMessageMethod(string ApiKey, string ChatId, string Message)
        {
            var endpoint = new Uri($"https://api.telegram.org/bot{ApiKey}/sendMessage");

            MultipartFormDataContent multipartFormContent = new MultipartFormDataContent();
            multipartFormContent.Add(new StringContent(ChatId), name: "chat_id");
            multipartFormContent.Add(new StringContent(Message), name: "text");

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
            if (MESSAGE == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(MESSAGE)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Object Container: Use objectContainer.Get<T>() to retrieve objects from the scope
            var objectContainer = context.GetFromContext<IObjectContainer>(TelegramScope.ParentContainerPropertyTag);

            // Inputs
            var chatID = CHATID.Get(context);
            var message = MESSAGE.Get(context);

            var property = context.DataContext.GetProperties()[TelegramScope.ParentContainerPropertyTag];
            var Container = property.GetValue(context.DataContext) as IObjectContainer;
            string apiKey = Container.Get<string>();

            // Add execution logic HERE
            SendMessage sMessage = new SendMessage();
            //await sMessage.sendMessageMethod(apiKey, chatID, message);
            Task<string> task = sMessage.sendMessageMethod(apiKey, chatID, message);
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

