using System;

namespace Common.Commons
{
    public static class ErrorConstants
    {
        #region Database Error Messages

        public static readonly string argumentNullExceptionMessageForDbContext = "DbContext objesi boş geçilemez!";

        public static readonly string argumentNullExceptionMessageForOrderDBContext = $"Lütfen ulaşmak istediğiniz tabloya ait repository nesnesi için" +
            $"tablonun bulunduğu DbContext nesnesini verin. Örn: OrderDBContext";

        #endregion Database Error Messages
    }
}
