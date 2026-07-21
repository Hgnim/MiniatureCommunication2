import ConversationType from './ConversationType.type';

type ConversationDTO_CrMember = {
    id: bigint,
    type: ConversationType,
    lastMessageId: bigint,
    group_Title:string,
}
export default ConversationDTO_CrMember;