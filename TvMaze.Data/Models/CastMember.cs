using System;

namespace TvMaze.Data.Models
{
    public class CastMember
    {
        public int MazeTvId { get; set; }
        public string Name { get; set; }
        public DateTime? Birthday { get; set; }

        #region Equality 

        protected bool Equals(CastMember other)
        {
            return MazeTvId == other.MazeTvId && Name == other.Name && Nullable.Equals(Birthday, other.Birthday);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CastMember)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MazeTvId, Name, Birthday);
        }

        #endregion
    }
}
