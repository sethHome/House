using System;
using System.Data;
using System.Configuration;
using System.Text;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using System.Collections.Generic;

namespace BPM.Engine
{
    public class ConditionExpression
    {
        /// <summary>     
        /// 用于动态引用生成的类，执行其内部包含的可执行字符串     
        /// </summary>     
        private object _Compiled = null;

        public static object Evaluator(string code,string className)
        {
            //创建C#编译器实例  
            CodeDomProvider provider = CodeDomProvider.CreateProvider("C#");

            //编译器的传入参数     
            CompilerParameters cp = new CompilerParameters();
            cp.ReferencedAssemblies.Add("system.dll");              //添加程序集 system.dll 的引用     
            cp.ReferencedAssemblies.Add("system.data.dll");         //添加程序集 system.data.dll 的引用     
            cp.ReferencedAssemblies.Add("system.xml.dll");          //添加程序集 system.xml.dll 的引用     
            cp.GenerateExecutable = false;                          //不生成可执行文件     
            cp.GenerateInMemory = true;                             //在内存中运行     

            //得到编译器实例的返回结果     
            CompilerResults cr = provider.CompileAssemblyFromSource(cp, code);//comp  

            if (cr.Errors.HasErrors)                            //如果有错误     
            {
                StringBuilder error = new StringBuilder();          //创建错误信息字符串     
                error.Append("编译有错误的表达式: ");                //添加错误文本     
                foreach (CompilerError err in cr.Errors)            //遍历每一个出现的编译错误     
                {
                    error.AppendFormat("{0}/n", err.ErrorText);     //添加进错误文本，每个错误后换行     
                }
                throw new Exception("编译错误: " + error.ToString());//抛出异常     
            }
            Assembly a = cr.CompiledAssembly;                       //获取编译器实例的程序集     
            return a.CreateInstance("BPM.Engine.ExpreScript." + className);     //通过程序集查找并声明 SSEC.Math._Evaluator 的实例     
        }

        /// <summary>     
        /// 执行字符串并返 object 型值     
        /// </summary>     
        /// <param name="name">执行字符串名称</param>     
        /// <returns>执行结果</returns>     
        public object Evaluate(string name, Dictionary<string, object> InputData)
        {
            MethodInfo mi = _Compiled.GetType().GetMethod(name);//获取 _Compiled 所属类型中名称为 name 的方法的引用     
            return mi.Invoke(_Compiled, new object[] { InputData });                  //执行 mi 所引用的方法     
        }
    }
}
