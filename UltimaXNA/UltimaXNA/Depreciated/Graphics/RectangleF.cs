using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace UltimaXNA.Graphics
{
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct RectangleF : IEquatable<RectangleF>
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        private static RectangleF _empty;

        public float Left
        {
            get
            {
                return this.X;
            }
        }

        public float Right
        {
            get
            {
                return (this.X + this.Width);
            }
        }

        public float Top
        {
            get
            {
                return this.Y;
            }
        }

        public float Bottom
        {
            get
            {
                return (this.Y + this.Height);
            }
        }

        public Vector2 Location
        {
            get
            {
                return new Vector2(this.X, this.Y);
            }
            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }

        public Vector2 Center
        {
            get
            {
                return new Vector2(this.X + (this.Width / 2), this.Y + (this.Height / 2));
            }
        }

        public static RectangleF Empty
        {
            get
            {
                return _empty;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return ((((this.Width == 0) && (this.Height == 0)) && (this.X == 0)) && (this.Y == 0));
            }
        }

        public RectangleF(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public RectangleF(Vector2 position, Vector2 size)
        {
            this.X = position.X;
            this.Y = position.Y;
            this.Width = size.X;
            this.Height = size.Y;
        }

        public void Offset(Vector2 amount)
        {
            this.X += amount.X;
            this.Y += amount.Y;
        }

        public void Offset(float offsetX, float offsetY)
        {
            this.X += offsetX;
            this.Y += offsetY;
        }

        public void Inflate(float horizontalAmount, float verticalAmount)
        {
            this.X -= horizontalAmount;
            this.Y -= verticalAmount;
            this.Width += horizontalAmount * 2;
            this.Height += verticalAmount * 2;
        }

        public bool Contains(float x, float y)
        {
            return ((((this.X <= x) && (x < (this.X + this.Width))) && (this.Y <= y)) && (y < (this.Y + this.Height)));
        }

        public bool Contains(Vector2 value)
        {
            return ((((this.X <= value.X) && (value.X < (this.X + this.Width))) && (this.Y <= value.Y)) && (value.Y < (this.Y + this.Height)));
        }

        public void Contains(ref Vector2 value, out bool result)
        {
            result = (((this.X <= value.X) && (value.X < (this.X + this.Width))) && (this.Y <= value.Y)) && (value.Y < (this.Y + this.Height));
        }

        public bool Contains(RectangleF value)
        {
            return ((((this.X <= value.X) && ((value.X + value.Width) <= (this.X + this.Width))) && (this.Y <= value.Y)) && ((value.Y + value.Height) <= (this.Y + this.Height)));
        }

        public void Contains(ref RectangleF value, out bool result)
        {
            result = (((this.X <= value.X) && ((value.X + value.Width) <= (this.X + this.Width))) && (this.Y <= value.Y)) && ((value.Y + value.Height) <= (this.Y + this.Height));
        }

        public bool Intersects(RectangleF value)
        {
            return ((((value.X < (this.X + this.Width)) && (this.X < (value.X + value.Width))) && (value.Y < (this.Y + this.Height))) && (this.Y < (value.Y + value.Height)));
        }

        public void Intersects(ref RectangleF value, out bool result)
        {
            result = (((value.X < (this.X + this.Width)) && (this.X < (value.X + value.Width))) && (value.Y < (this.Y + this.Height))) && (this.Y < (value.Y + value.Height));
        }

        public static RectangleF Intersect(RectangleF value1, RectangleF value2)
        {
            RectangleF rectangle;
            float num8 = value1.X + value1.Width;
            float num7 = value2.X + value2.Width;
            float num6 = value1.Y + value1.Height;
            float num5 = value2.Y + value2.Height;
            float num2 = (value1.X > value2.X) ? value1.X : value2.X;
            float num = (value1.Y > value2.Y) ? value1.Y : value2.Y;
            float num4 = (num8 < num7) ? num8 : num7;
            float num3 = (num6 < num5) ? num6 : num5;
            if ((num4 > num2) && (num3 > num))
            {
                rectangle.X = num2;
                rectangle.Y = num;
                rectangle.Width = num4 - num2;
                rectangle.Height = num3 - num;
                return rectangle;
            }
            rectangle.X = 0;
            rectangle.Y = 0;
            rectangle.Width = 0;
            rectangle.Height = 0;
            return rectangle;
        }

        public static void Intersect(ref RectangleF value1, ref RectangleF value2, out RectangleF result)
        {
            float num8 = value1.X + value1.Width;
            float num7 = value2.X + value2.Width;
            float num6 = value1.Y + value1.Height;
            float num5 = value2.Y + value2.Height;
            float num2 = (value1.X > value2.X) ? value1.X : value2.X;
            float num = (value1.Y > value2.Y) ? value1.Y : value2.Y;
            float num4 = (num8 < num7) ? num8 : num7;
            float num3 = (num6 < num5) ? num6 : num5;
            if ((num4 > num2) && (num3 > num))
            {
                result.X = num2;
                result.Y = num;
                result.Width = num4 - num2;
                result.Height = num3 - num;
            }
            else
            {
                result.X = 0;
                result.Y = 0;
                result.Width = 0;
                result.Height = 0;
            }
        }

        public static RectangleF Union(RectangleF value1, RectangleF value2)
        {
            RectangleF rectangle;
            float num6 = value1.X + value1.Width;
            float num5 = value2.X + value2.Width;
            float num4 = value1.Y + value1.Height;
            float num3 = value2.Y + value2.Height;
            float num2 = (value1.X < value2.X) ? value1.X : value2.X;
            float num = (value1.Y < value2.Y) ? value1.Y : value2.Y;
            float num8 = (num6 > num5) ? num6 : num5;
            float num7 = (num4 > num3) ? num4 : num3;
            rectangle.X = num2;
            rectangle.Y = num;
            rectangle.Width = num8 - num2;
            rectangle.Height = num7 - num;
            return rectangle;
        }

        public static void Union(ref RectangleF value1, ref RectangleF value2, out RectangleF result)
        {
            float num6 = value1.X + value1.Width;
            float num5 = value2.X + value2.Width;
            float num4 = value1.Y + value1.Height;
            float num3 = value2.Y + value2.Height;
            float num2 = (value1.X < value2.X) ? value1.X : value2.X;
            float num = (value1.Y < value2.Y) ? value1.Y : value2.Y;
            float num8 = (num6 > num5) ? num6 : num5;
            float num7 = (num4 > num3) ? num4 : num3;
            result.X = num2;
            result.Y = num;
            result.Width = num8 - num2;
            result.Height = num7 - num;
        }

        public bool Equals(RectangleF other)
        {
            return ((((this.X == other.X) && (this.Y == other.Y)) && (this.Width == other.Width)) && (this.Height == other.Height));
        }

        public override bool Equals(object obj)
        {
            bool flag = false;
            if (obj is RectangleF)
            {
                flag = this.Equals((RectangleF)obj);
            }
            return flag;
        }

        public override string ToString()
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            return string.Format(currentCulture, "{{X:{0} Y:{1} Width:{2} Height:{3}}}", new object[] { this.X.ToString(currentCulture), this.Y.ToString(currentCulture), this.Width.ToString(currentCulture), this.Height.ToString(currentCulture) });
        }

        public override int GetHashCode()
        {
            return (((this.X.GetHashCode() + this.Y.GetHashCode()) + this.Width.GetHashCode()) + this.Height.GetHashCode());
        }

        public static bool operator ==(RectangleF a, RectangleF b)
        {
            return ((((a.X == b.X) && (a.Y == b.Y)) && (a.Width == b.Width)) && (a.Height == b.Height));
        }

        public static bool operator !=(RectangleF a, RectangleF b)
        {
            if (((a.X == b.X) && (a.Y == b.Y)) && (a.Width == b.Width))
            {
                return (a.Height != b.Height);
            }
            return true;
        }
        
        public static implicit operator Rectangle(RectangleF rect)
        {
            return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }

        public static implicit operator RectangleF(Rectangle rect)
        {
            return new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }

        static RectangleF()
        {
            _empty = new RectangleF();
        }

        public void GetSize(out Vector2 size)
        {
            size.X = Width;
            size.Y = Height;
        }

        public void GetPosition(out Vector2 position)
        {
            position.X = X;
            position.Y = Y;
        }
    }
}
