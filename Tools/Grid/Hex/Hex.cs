using System;

public struct Hex
{
    public readonly int Q;
    public readonly int R;
    public readonly int S;

    private static readonly Hex[] diagonals = new Hex[] {new Hex(2, -1, -1), new Hex(1, -2, 1), new Hex(-1, -1, 2), new Hex(-2, 1, 1), new Hex(-1, 2, -1), new Hex(1, 1, -2)};
    private static readonly Hex[] directions = new Hex[] {new Hex(1, 0, -1), new Hex(1, -1, 0), new Hex(0, -1, 1), new Hex(-1, 0, 1), new Hex(-1, 1, 0), new Hex(0, 1, -1)};
    
    public Hex(int q, int r, int s)
    {
        this.Q = q;
        this.R = r;
        this.S = s;
        if (q + r + s != 0) throw new ArgumentException("q + r + s must be 0");
    }
    
    public Hex(int q, int r)
    {
        this.Q = q;
        this.R = r;
        this.S = -q -r;
        if (q + r + S != 0) throw new ArgumentException("q + r + s must be 0");
    }

    public Hex Add(Hex b)
    {
        return new Hex(Q + b.Q, R + b.R, S + b.S);
    }

    public Hex Subtract(Hex b)
    {
        return new Hex(Q - b.Q, R - b.R, S - b.S);
    }
    
    public Hex Scale(int k)
    {
        return new Hex(Q * k, R * k, S * k);
    }

    public Hex RotateLeft()
    {
        return new Hex(-S, -Q, -R);
    }

    public Hex RotateRight()
    {
        return new Hex(-R, -S, -Q);
    }

    static public Hex Direction(int direction)
    {
        return Hex.directions[direction];
    }

    public Hex Neighbor(int direction)
    {
        return Add(Hex.Direction(direction));
    }

    public Hex DiagonalNeighbor(int direction)
    {
        return Add(Hex.diagonals[direction]);
    }

    public int Length()
    {
        return (int)((Math.Abs(Q) + Math.Abs(R) + Math.Abs(S)) / 2);
    }

    public int Distance(Hex b)
    {
        return Subtract(b).Length();
    }
    
    public override bool Equals(object other)
    {
        if (!(other is Hex))
        {
            return false;
        }
        Hex hex = (Hex)other;
        return Q.Equals(hex.Q) && R.Equals(hex.R) && S.Equals(hex.S);
    }

    public override int GetHashCode()
    {
        int hashCode1 = R.GetHashCode();
        int hashCode2 = S.GetHashCode();
        return this.Q.GetHashCode() ^ hashCode1 << 4 ^ hashCode1 >> 28 ^ hashCode2 >> 4 ^ hashCode2 << 28;
    }

    public override string ToString()
    {
        return string.Format("({0:F1}, {1:F1},  {2:F1})", Q, R, S);
    }
    
    public static Hex operator +(Hex a, Hex b)
    {
        return new Hex(a.Q + b.Q, a.Q + b.Q, a.S + a.S);
    }

    public static Hex operator -(Hex a, Hex b)
    {
        return new Hex(a.Q - b.Q, a.R - b.R, a.S - b.S);
    }
    
    public static FractionalHex operator *(Hex a, float d)
    {
        return new FractionalHex(a.Q * d, a.R * d, a.S * d);
    }

    public static FractionalHex operator *(float d, Hex a)
    {
        return new FractionalHex(a.Q * d, a.R * d, a.S * d);
    }

    public static FractionalHex operator /(Hex a, float d)
    {
        return new FractionalHex(a.Q / d, a.R / d, a.S / d);
    }

    public static bool operator ==(Hex lhs, Hex rhs)
    {
        return lhs.Q == rhs.Q && lhs.R == rhs.R && lhs.S == rhs.S;
    }

    public static bool operator !=(Hex lhs, Hex rhs)
    {
        return !(lhs == rhs);
    }
}
