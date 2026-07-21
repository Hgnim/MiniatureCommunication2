using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniatureCommunication2.Database {
	public enum ConversationType {
		/// <summary>
		/// 一对一会话（私聊）
		/// </summary>
		One2One,
		/// <summary>
		/// 群组会话（群聊）
		/// </summary>
		Group,
	}
	public enum ConversationRole {
		/// <summary>
		/// 访客，没有发送消息的权限，只能接受消息
		/// </summary>
		Visitor,
		/// <summary>
		/// 成员，可以发送与接受消息
		/// </summary>
		Member,
		/// <summary>
		/// 所有者，拥有该群组的管理权限
		/// </summary>
		Owner,
	}
	public enum ConversationMessageType {
		/// <summary>
		/// 纯文本消息
		/// </summary>
		Text,
		/// <summary>
		/// markdown消息
		/// </summary>
		Markdown,
	}
	/// <summary>
	/// 会话实体类
	/// <br/>
	/// 表示一个会话
	/// </summary>
	public class Conversation {
		/// <summary>
		/// 会话唯一标识
		/// 主键
		/// 自增
		/// </summary>
		[Key]//主键
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增
		public long Id { get; set; }
		/// <summary>
		/// 会话类型
		/// </summary>
		public required ConversationType Type { get; set; }
		/// <summary>
		/// 最后一条消息的id
		/// </summary>
		public long? LastMessageId { get; set; }
		/// <summary>
		/// 创建会话时间，可能冗余
		/// </summary>
		public DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;


		//下列属性只在群组会话中使用，一对一会话中为null
		/// <summary>
		/// 会话标题
		/// </summary>
		public string? Group_Title { get; set; }
		/// <summary>
		/// 是否禁用显示成员列表
		/// </summary>
		public bool? Group_DisShowUserList { get; set; }
		/// <summary>
		/// 在注册时是否强制用户加入该群组
		/// </summary>
		public bool? Group_ForceUserJoinOnReg { get; set; }
	}
	/// <summary>
	/// 会话成员实体类
	/// <br/>
	/// 表示某一个会话中的某一个成员
	/// </summary>
	public class ConversationMember {
		/// <summary>
		/// 成员记录唯一标识
		/// 主键
		/// 自增
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }
		/// <summary>
		/// 用户id
		/// <br/>
		/// 该值对应MiniatureCommunication2.Database.IdentityUser表中的Id
		/// </summary>
		public required string UserId { get; set; }
		/// <summary>
		/// 所属会话的id
		/// <br/>
		/// 该值对应Conversation表中的Id
		/// </summary>
		public required long ConversationId { get; set; }
		/// <summary>
		/// 当前成员在该会话中的角色
		/// </summary>
		public ConversationRole Role { get; set; } = ConversationRole.Member;
		/// <summary>
		/// 加入会话的时间，可能冗余
		/// </summary>
		public DateTimeOffset JoinedAt { get; set; } = DateTime.UtcNow;
		/// <summary>
		/// 最后阅读的消息id
		/// </summary>
		public long? LastReadMessageId { get; set; }
	}
	/// <summary>
	/// 会话消息实体类
	/// <br/>
	/// 表示某一个会话中的某一条消息
	/// </summary>
	public class ConversationMessage {
		/// <summary>
		/// 消息记录唯一标识
		/// 主键
		/// 自增
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }
		/// <summary>
		/// 发送者的用户id
		/// <br/>
		/// 该值对应MiniatureCommunication2.Database.IdentityUser表中的Id
		/// </summary>
		public required string SenderId { get; set; }
		/// <summary>
		/// 所属会话的id
		/// <br/>
		/// 该值对应Conversation表中的Id
		/// </summary>
		public required long ConversationId { get; set; }
		/// <summary>
		/// 消息类型，决定了前端如何渲染
		/// </summary>
		public ConversationMessageType Type { get; set; } = ConversationMessageType.Text;
		/// <summary>
		/// 消息内容，具体格式根据Type决定
		/// </summary>
		public required string Content { get; set; }
		/// <summary>
		/// 发送消息时间
		/// </summary>
		public DateTimeOffset SentAt { get; set; } = DateTime.UtcNow;
		/// <summary>
		/// 回复的消息id
		/// <br/>
		/// 该值对应ConversationMessage表中的Id
		/// <br/>
		/// 如果该消息不是回复消息，则为null
		/// </summary>
		public long? ReplyToMsgId { get; set; }
	}
}
