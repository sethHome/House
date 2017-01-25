using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BPM.Engine
{
    public class SequenceInstance : TaskInstance
    {
        public string Condition { get; set; }

        public override async Task<string> Excute(ProcessInstance pi)
        {
            return await Task<string>.Factory.StartNew(() =>
            {
                if (string.IsNullOrEmpty(this.Condition))
                {
                    return this.To;
                }

                var val = this.Condition;

                if (this.Condition.StartsWith("${") && this.Condition.EndsWith("}"))
                {
                    val = this.Condition.Substring(2).TrimEnd('}');
                }

                if (pi.Compiled != null)
                {
                    MethodInfo mi = pi.Compiled.GetType().GetMethod(val);    //获取 _Compiled 所属类型中名称为 name 的方法的引用     
                    var result = mi.Invoke(pi.Compiled, new object[] { pi.InputData });            //执行 mi 所引用的方法    

                    if ((bool)result)
                    {
                        return this.To;
                    }

                    return null;
                }
                else
                {
                    if (val.StartsWith("!"))
                    {
                        if (!(bool)pi.InputData[val.TrimStart('!')])
                        {
                            return this.To;
                        }
                    }
                    else
                    {
                        if ((bool)pi.InputData[val])
                        {
                            return this.To;
                        }
                    }


                    return null;
                }
            });
        }
    }
}
