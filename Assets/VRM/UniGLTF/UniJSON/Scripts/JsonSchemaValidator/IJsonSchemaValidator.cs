using System;


namespace UniJSON
{
    public class JsonSchemaValidationContext
    {
        string[] m_stack = new string[64];
        //int m_pos;

        public JsonSchemaValidationContext(object o)
        {
            Push(o.GetType().Name);
        }

        public void Push(object o)
        {
            return;
            /*
            if (m_pos >= m_stack.Length)
            {
                // extend array
                // but may be forget pop
                var stack = new string[m_stack.Length * 2];
                Array.Copy(m_stack, stack, m_stack.Length);
                m_stack = stack;
            }
            m_stack[m_pos++] = o.ToString();
            return new ActionDisposer(Pop);
            */
        }

        public void Pop()
        {
            //--m_pos;
        }

        public override string ToString()
        {
            return "";
            //return string.Join(".", m_stack, 0, m_pos);
        }
    }


    public class JsonSchemaValidationException : Exception
    {
        public Exception Error
        {
            get; private set;
        }

        public JsonSchemaValidationException(JsonSchemaValidationContext context, string msg) : base(string.Format("[{0}] {1}", context, msg))
        {
        }

        public JsonSchemaValidationException(JsonSchemaValidationContext context, Exception ex) : base(string.Format("[{0}] {1}", context, ex))
        {
            Error = ex;
        }
    }


    public interface IJsonSchemaValidator
    {
        #region JsonSchema
        void Merge(IJsonSchemaValidator rhs);

        /// <summary>
        /// Parse json schema
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool FromJsonSchema(IFileSystemAccessor fs, string key, ListTreeNode<JsonValue> value);

        void ToJsonScheama(IFormatter f);
        #endregion

        #region Serializer
        /// <summary>
        ///
        /// </summary>
        /// <param name="o"></param>
        /// <returns>return null if validate value</returns>
        JsonSchemaValidationException Validate<T>(JsonSchemaValidationContext context, T value);

        void Serialize<T>(IFormatter f, JsonSchemaValidationContext context, T value);

        void Deserialize<T, U>(ListTreeNode<T> src, ref U dst) where T : IListTreeItem, IValue<T>;
        #endregion
    }
}
