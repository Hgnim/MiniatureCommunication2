using YamlDotNet.Serialization;

namespace WebLoginDemo.Models {
	public class ConfigModel {
		/// <summary>
		/// 将配置数据保存至配置文件中
		/// </summary>
		/// <param name="configFile">配置文件路径</param>
		/// <param name="config">配置数据</param>
		internal static void SaveData(string configFile, ConfigModel config) {
			ISerializer yamlS = new SerializerBuilder()
					.Build();
			File.WriteAllText(configFile, yamlS.Serialize(config));
		}
		/// <summary>
		/// 读取数据文件并将数据写入实例中
		/// </summary>
		/// <param name="configFile">配置文件路径</param>
		internal static ConfigModel ReadData(string configFile) {
			IDeserializer yamlD = new DeserializerBuilder()
					.Build();
			return yamlD.Deserialize<ConfigModel>(File.ReadAllText(configFile));
		}


		public class WebsiteC {

			private bool useXFFRequestHeader = false;
			/// <summary>
			/// 是否启用 X-Forwarded-For(XFF)请求标头
			/// </summary>
			public bool UseXFFRequestHeader {
				set => useXFFRequestHeader = value;
				get => useXFFRequestHeader;
			}
			public class UrlC {

				private bool useHttps = false;
				/// <summary>
				/// 使用https
				/// </summary>
				public bool UseHttps {
					set => useHttps = value;
					get => useHttps;
				}

				private string addr = "*";
				/// <summary>
				/// 本地监听地址
				/// </summary>
				public string Addr {
					set => addr = value;
					get => addr;
				}
				///<summary>
				///urlRoot不能只包含单独的斜杠，这里只是起到占位的作用，到引用的时候单独的斜杠会被去掉。<br/>
				///在包含内容的时候，urlRoot前面必须包含斜杠，末尾不能含有斜杠
				///</summary>
				private string urlRoot = "/";
				/// <summary>
				/// 主机地址后的URL地址
				/// </summary>
				public string UrlRoot {
					set {//格式化
						string urlRoot_;
						if (value == "/") urlRoot_ = "";//value不能只包含单独的斜杠
						else if (value == "") { urlRoot_ = value; }//如果为空则直接输出
						else {
							if (value[..1] == "/")
								urlRoot_ = value;
							else//如果开头没有斜杠则加上斜杠
								urlRoot_ = "/" + value;
							if (urlRoot_.Substring(urlRoot_.Length - 1, 1) == "/")//如果末尾包含斜杠则去掉
								urlRoot_ = urlRoot_[..(urlRoot_.Length - 1)];
						}
						urlRoot = urlRoot_;
					}
					get => urlRoot;
				}
				private string port = "80";
				/// <summary>
				/// 监听端口
				/// </summary>
				public string Port {
					set => port = value;
					get => port;
				}
				internal string Get() {
					string head;

					if (useHttps)
						head = "https";
					else
						head = "http";

					return $"{head}://{addr}:{port}";
				}
			}
			private UrlC url = new();
			public UrlC Url {
				set => url = value;
				get => url;
			}


			public class DatabaseC {
				private string connectionString = /*Data Source=*/"wld_data/data.db";
				/// <summary>
				/// 数据库连接字符串
				/// </summary>
				public string ConnectionString {
					set => connectionString = value;
					get => connectionString;
				}
			}
			private DatabaseC database = new();
			public DatabaseC Database {
				set => database = value;
				get => database;
			}
		}
		private WebsiteC website = new();
		public WebsiteC Website {
			set => website = value;
			get => website;
		}


		private bool updateConfig = true;
		/// <summary>
		/// 设置为true后将在下次启动时更新一次配置文件
		/// </summary>
		public bool UpdateConfig {
			get => updateConfig;
			set => updateConfig = value;
		}
	}
}
