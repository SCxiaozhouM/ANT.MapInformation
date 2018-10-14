namespace ANT.MapInformation.WebAPI.Models
{
    /// <summary>
    /// 地图信息list模型
    /// </summary>
    public class MarkersListModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///纬度
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; set; }
        public string OpenId { get; set; }
    }
}