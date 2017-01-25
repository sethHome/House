namespace Api.Framework.Core
{
    public class SysUserEntity
    {
        public int ID { get; set; }

        public string Account { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public bool Visiable { get; set; }

        public string PhotoImg { get; set; }

        public string PhotoImgLarge { get; set; }
    }
}
