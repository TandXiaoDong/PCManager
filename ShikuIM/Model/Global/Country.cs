using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Linq;

namespace ShikuIM.Model
{

    /// <summary>
    /// 国际化时
    /// </summary>
    public class Country : ObservableObject
    {

        #region Private Member
        private int _id;
        private string _enName;
        private string _country;
        private int _prefix;
        private double _price;
        #endregion

        #region Public Member

        /// <summary>
        /// ID
        /// </summary>
        public int id
        {
            get { return _id; }
            set { _id = value; RaisePropertyChanged(nameof(id)); }
        }

        /// <summary>
        /// 英语名称
        /// </summary>
        public string enName
        {
            get { return _enName; }
            set { _enName = value; RaisePropertyChanged(nameof(enName)); }
        }

        /// <summary>
        /// 中文名称
        /// </summary>
        public string country
        {
            get { return _country; }
            set { _country = value; RaisePropertyChanged(nameof(country)); }
        }


        /// <summary>
        /// 区号
        /// </summary>
        public int prefix
        {
            get { return _prefix; }
            set { _prefix = value; RaisePropertyChanged(nameof(prefix)); }
        }

        /// <summary>
        /// Price ？
        /// </summary>
        public double price
        {
            get { return _price; }
            set { _price = value; RaisePropertyChanged(nameof(price)); }
        }

        #endregion

        #region Public Method

        public static List<Country> GetCountries()
        {
            ConstantDBContext.DBAutoConnect();
            var countries = new List<Country>();
            //Applicate.ConstantDBContext
            countries = (from country in Applicate.ConstantDBContext.Countrys
                       where country.id > 0 orderby country.country ascending
                       select country).ToList();
            return countries;
        }
        #endregion



    }
}
