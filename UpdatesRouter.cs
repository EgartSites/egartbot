using System.Reflection;
using TdLib;

namespace egartbot
{
    public sealed class UpdatesRouter : Program
    {
        public delegate Task UpdateHandler<T>(T update);

#pragma warning disable CS0067, IDE0051
        private event UpdateHandler<TdApi.Update.UpdateActiveNotifications>? UpdateActiveNotifications;
        private event UpdateHandler<TdApi.Update.UpdateAnimatedEmojiMessageClicked>? UpdateAnimatedEmojiMessageClicked;
        private event UpdateHandler<TdApi.Update.UpdateAnimationSearchParameters>? UpdateAnimationSearchParameters;
        private event UpdateHandler<TdApi.Update.UpdateAuthorizationState>? UpdateAuthorizationState;
        private event UpdateHandler<TdApi.Update.UpdateBasicGroup>? UpdateBasicGroup;
        private event UpdateHandler<TdApi.Update.UpdateBasicGroupFullInfo>? UpdateBasicGroupFullInfo;
        private event UpdateHandler<TdApi.Update.UpdateCall>? UpdateCall;
        private event UpdateHandler<TdApi.Update.UpdateChatAction>? UpdateChatAction;
        private event UpdateHandler<TdApi.Update.UpdateChatActionBar>? UpdateChatActionBar;
        private event UpdateHandler<TdApi.Update.UpdateChatDefaultDisableNotification>? UpdateChatDefaultDisableNotifications;
        private event UpdateHandler<TdApi.Update.UpdateChatDraftMessage>? UpdateChatDraftMessage;
        private event UpdateHandler<TdApi.Update.UpdateChatFilters>? UpdateChatFilters;
        private event UpdateHandler<TdApi.Update.UpdateChatHasProtectedContent>? UpdateChatHasProtectedContent;
        private event UpdateHandler<TdApi.Update.UpdateChatHasScheduledMessages>? UpdateChatHasScheduledMessages;
        private event UpdateHandler<TdApi.Update.UpdateChatIsBlocked>? UpdateChatIsBlocked;
        private event UpdateHandler<TdApi.Update.UpdateChatIsMarkedAsUnread>? UpdateChatIsMarkedAsUnread;
        private event UpdateHandler<TdApi.Update.UpdateChatLastMessage>? UpdateChatLastMessage;
        private event UpdateHandler<TdApi.Update.UpdateChatMember>? UpdateChatMember;
        private event UpdateHandler<TdApi.Update.UpdateChatMessageSender>? UpdateChatMessageSender;
        private event UpdateHandler<TdApi.Update.UpdateChatNotificationSettings>? UpdateChatNotificationSettings;
        private event UpdateHandler<TdApi.Update.UpdateChatOnlineMemberCount>? UpdateChatOnlineMemberCount;
        private event UpdateHandler<TdApi.Update.UpdateChatPendingJoinRequests>? UpdateChatPendingJoinRequests;
        private event UpdateHandler<TdApi.Update.UpdateChatPermissions>? UpdateChatPermissions;
        private event UpdateHandler<TdApi.Update.UpdateChatPhoto>? UpdateChatPhoto;
        private event UpdateHandler<TdApi.Update.UpdateChatPosition>? UpdateChatPosition;
        private event UpdateHandler<TdApi.Update.UpdateChatReadInbox>? UpdateChatReadInbox;
        private event UpdateHandler<TdApi.Update.UpdateChatReadOutbox>? UpdateChatReadOutbox;
        private event UpdateHandler<TdApi.Update.UpdateChatReplyMarkup>? UpdateChatReplyMarkup;
        private event UpdateHandler<TdApi.Update.UpdateChatTheme>? UpdateChatTheme;
        private event UpdateHandler<TdApi.Update.UpdateChatThemes>? UpdateChatThemes;
        private event UpdateHandler<TdApi.Update.UpdateChatTitle>? UpdateChatTitle;
        private event UpdateHandler<TdApi.Update.UpdateChatUnreadMentionCount>? UpdateChatUnreadMentionCount;
        private event UpdateHandler<TdApi.Update.UpdateChatVideoChat>? UpdateChatVideoChat;
        private event UpdateHandler<TdApi.Update.UpdateConnectionState>? UpdateConnectionState;
        private event UpdateHandler<TdApi.Update.UpdateDeleteMessages>? UpdateDeleteMessages;
        private event UpdateHandler<TdApi.Update.UpdateDiceEmojis>? UpdateDiceEmojis;
        private event UpdateHandler<TdApi.Update.UpdateFavoriteStickers>? UpdateFavoriteStickers;
        private event UpdateHandler<TdApi.Update.UpdateFile>? UpdateFile;
        private event UpdateHandler<TdApi.Update.UpdateFileGenerationStart>? UpdateFileGenerationStart;
        private event UpdateHandler<TdApi.Update.UpdateFileGenerationStop>? UpdateFileGenerationStop;
        private event UpdateHandler<TdApi.Update.UpdateGroupCall>? UpdateGroupCall;
        private event UpdateHandler<TdApi.Update.UpdateGroupCallParticipant>? UpdateGroupCallParticipant;
        private event UpdateHandler<TdApi.Update.UpdateHavePendingNotifications>? UpdateHavePendingNotifications;
        private event UpdateHandler<TdApi.Update.UpdateInstalledStickerSets>? UpdateInstalledStickerSets;
        private event UpdateHandler<TdApi.Update.UpdateLanguagePackStrings>? UpdateLanguagePackStrings;
        private event UpdateHandler<TdApi.Update.UpdateMessageContent>? UpdateMessageContent;
        private event UpdateHandler<TdApi.Update.UpdateMessageContentOpened>? UpdateMessageContentOpened;
        private event UpdateHandler<TdApi.Update.UpdateMessageEdited>? UpdateMessageEdited;
        private event UpdateHandler<TdApi.Update.UpdateMessageInteractionInfo>? UpdateMessageInteractionInfo;
        private event UpdateHandler<TdApi.Update.UpdateMessageIsPinned>? UpdateMessageIsPinned;
        private event UpdateHandler<TdApi.Update.UpdateMessageLiveLocationViewed>? UpdateMessageLiveLocationViewed;
        private event UpdateHandler<TdApi.Update.UpdateMessageMentionRead>? UpdateMessageMentionRead;
        private event UpdateHandler<TdApi.Update.UpdateMessageSendAcknowledged>? UpdateMessageSendAcknowledged;
        private event UpdateHandler<TdApi.Update.UpdateMessageSendFailed>? UpdateMessageSendFailed;
        private event UpdateHandler<TdApi.Update.UpdateMessageSendSucceeded>? UpdateMessageSendSucceeded;
        private event UpdateHandler<TdApi.Update.UpdateNewCallbackQuery>? UpdateNewCallbackQuery;
        private event UpdateHandler<TdApi.Update.UpdateNewCallSignalingData>? UpdateNewCallSignalingData;
        private event UpdateHandler<TdApi.Update.UpdateNewChat>? UpdateNewChat;
        private event UpdateHandler<TdApi.Update.UpdateNewChatJoinRequest>? UpdateNewChatJoinRequest;
        private event UpdateHandler<TdApi.Update.UpdateNewChosenInlineResult>? UpdateNewChosenInlineResult;
        private event UpdateHandler<TdApi.Update.UpdateNewCustomEvent>? UpdateNewCustomEvent;
        private event UpdateHandler<TdApi.Update.UpdateNewCustomQuery>? UpdateNewCustomQuery;
        private event UpdateHandler<TdApi.Update.UpdateNewInlineCallbackQuery>? UpdateNewInlineCallbackQuery;
        private event UpdateHandler<TdApi.Update.UpdateNewInlineQuery>? UpdateNewInlineQuery;
        private event UpdateHandler<TdApi.Update.UpdateNewMessage>? UpdateNewMessage;
        private event UpdateHandler<TdApi.Update.UpdateNewPreCheckoutQuery>? UpdateNewPreCheckoutQuery;
        private event UpdateHandler<TdApi.Update.UpdateNewShippingQuery>? UpdateNewShippingQuery;
        private event UpdateHandler<TdApi.Update.UpdateNotification>? UpdateNotification;
        private event UpdateHandler<TdApi.Update.UpdateNotificationGroup>? UpdateNotificationGroup;
        private event UpdateHandler<TdApi.Update.UpdateOption>? UpdateOption;
        private event UpdateHandler<TdApi.Update.UpdatePoll>? UpdatePoll;
        private event UpdateHandler<TdApi.Update.UpdatePollAnswer>? UpdatePollAnswer;
        private event UpdateHandler<TdApi.Update.UpdateRecentStickers>? UpdateRecentStickers;
        private event UpdateHandler<TdApi.Update.UpdateSavedAnimations>? UpdateSavedAnimations;
        private event UpdateHandler<TdApi.Update.UpdateScopeNotificationSettings>? UpdateScopeNotificationSettings;
        private event UpdateHandler<TdApi.Update.UpdateSecretChat>? UpdateSecretChat;
        private event UpdateHandler<TdApi.Update.UpdateSelectedBackground>? UpdateSelectedBackground;
        private event UpdateHandler<TdApi.Update.UpdateServiceNotification>? UpdateServiceNotification;
        private event UpdateHandler<TdApi.Update.UpdateStickerSet>? UpdateStickerSet;
        private event UpdateHandler<TdApi.Update.UpdateSuggestedActions>? UpdateSuggestedActions;
        private event UpdateHandler<TdApi.Update.UpdateSupergroup>? UpdateSupergroup;
        private event UpdateHandler<TdApi.Update.UpdateSupergroupFullInfo>? UpdateSupergroupFullInfo;
        private event UpdateHandler<TdApi.Update.UpdateTermsOfService>? UpdateTermsOfService;
        private event UpdateHandler<TdApi.Update.UpdateTrendingStickerSets>? UpdateTrendingStickerSets;
        private event UpdateHandler<TdApi.Update.UpdateUnreadChatCount>? UpdateUnreadChatCount;
        private event UpdateHandler<TdApi.Update.UpdateUnreadMessageCount>? UpdateUnreadMessageCount;
        private event UpdateHandler<TdApi.Update.UpdateUser>? UpdateUser;
        private event UpdateHandler<TdApi.Update.UpdateUserFullInfo>? UpdateUserFullInfo;
        private event UpdateHandler<TdApi.Update.UpdateUserPrivacySettingRules>? UpdateUserPrivacySettingRules;
        private event UpdateHandler<TdApi.Update.UpdateUsersNearby>? UpdateUsersNearby;
        private event UpdateHandler<TdApi.Update.UpdateUserStatus>? UpdateUserStatus;
#pragma warning restore CS0067, IDE0051

        private readonly EventInfo[] events;
        private readonly FieldInfo[] fields;

        private readonly string SelfModuleName;
        internal UpdatesRouter()
        {
            events = GetType().GetEvents(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            SelfModuleName = GetType().Module.Name;
        }

        internal void Subscribe<T>(UpdateHandler<T> toSubscribe, object source, string caller = "")
        {
            var sourceType = source.GetType();
            var updateType = typeof(T).Name;

            if (loadedModules.ContainsKey(sourceType.Name) || sourceType.Module.Name == SelfModuleName)
            {
                if (sourceType.Module.Name != SelfModuleName)
                {
                    if (!loadedModules[sourceType.Name].SubscribedToUpdates.Add(updateType))
                        return;
                }

                var eventHandler = events.FirstOrDefault(e => e.Name == updateType);

                if (eventHandler == null)
                    return;

                eventHandler.AddMethod?.Invoke(this, new object[] { toSubscribe });
            }
        }


        internal void Unsubscribe(string updateType, string moduleName)
        {
            var eventHandler = events.FirstOrDefault(e => e.Name == updateType);

            if (eventHandler == null)
                return;

            var target = fields.FirstOrDefault(f => f.Name == updateType)?.GetValue(this);

            if (target == null)
                return;

            var getInvocationList = target.GetType().GetMethod("GetInvocationList");

            Delegate[]? invocationList = (Delegate[]?)getInvocationList?.Invoke(target, Array.Empty<object>());

            var toUnsubscibe = invocationList?.FirstOrDefault(d => d.Method.DeclaringType?.Name == moduleName);

            if (toUnsubscibe == null)
                return;

            eventHandler.RemoveMethod?.Invoke(this, new object[] { toUnsubscibe });
        }


        internal void Route(TdApi.Update update)
        { 
            var name = update.GetType().Name;

            var target = fields.FirstOrDefault(f => f.Name == name)?.GetValue(this);

            if (target == null)
                return;

            var invoke = target.GetType().GetMethod("Invoke");

            invoke?.Invoke(target, new object?[] { update });
        }

    }
}

