using System;
using System.Text;

namespace Tast.Entities
{
	/// <summary>
	/// 结果
	/// </summary>
	public class Result
	{
		/// <summary>
		/// 成功与否
		/// </summary>
		public bool Success { get; set; }

		/// <summary>
		/// 显示信息
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// 数据
		/// </summary>
		public object Data { get; set; }

		/// <summary>
		/// 操作成功
		/// </summary>
		public static Result OK = new Result { Success = true, Message = "操作成功" };

		/// <summary>
		/// 参数为空
		/// </summary>
		public static Result ArgumentNull = new Result { Success = false, Message = "参数为空" };

		/// <summary>
		/// 记录未找到
		/// </summary>
		public static Result RecordNotFound = new Result { Success = false, Message = "记录未找到" };

		/// <summary>
		/// 创建结果
		/// </summary>
		/// <param name="success"></param>
		/// <returns></returns>
		public static Result Create(bool success)
		{
			return Create(success, null);
		}

		/// <summary>
		/// 创建结果
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static Result Create(object data)
		{
			return Create(data != null, data);
		}

		/// <summary>
		/// 创建结果
		/// </summary>
		/// <param name="success"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public static Result Create(bool success, string message)
		{
			return Create(success, message, null);
		}

		/// <summary>
		/// 创建结果
		/// </summary>
		/// <param name="success"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public static Result Create(bool success, object data)
		{
			return Create(success, null, data);
		}

		/// <summary>
		/// 创建结果
		/// </summary>
		/// <param name="success"></param>
		/// <param name="message"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public static Result Create(bool success, string message, object data)
		{
			return new Result { Success = success, Message = message, Data = data };
		}

		/// <summary>
		/// 根据异常创建结果
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		public static Result Create(Exception ex)
		{
			return new Result { Success = false, Message = ex.Message };
		}

		public override string ToString()
		{
			return string.IsNullOrEmpty(Message) ? string.Format("操作{0} ", Success ? "成功" : "失败") : Message;
		}
	}
}