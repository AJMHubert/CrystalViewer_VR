using System;
using System.Collections.Generic;

//template <class T>
//class Coord3D
//{
//public:
//    Coord3D(){};
//    Coord3D(T xin, T yin, T zin) : x(xin), y(yin), z(zin) {}
//
//    T x ,y, z;
//
//    bool operator==(const Coord3D<T>& rhs)
//    {
//        double accuracy = 1.0e-12;
//        // might want something to determine the accuracy of the ==
//        return (std::abs(x - rhs.x) < accuracy) && (std::abs(y - rhs.y) < accuracy) && (std::abs(z - rhs.z) < accuracy);
//    }
//};

public static class Utilities
{
	public static List<string> split(string s, sbyte delim, List<string> elems)
	{
		std.stringstream ss = new std.stringstream(s);
		string item;
		while (getline(ss, item, delim))
		{
			elems.Add(item);
		}
		return elems;
	}


	public static List<string> split(string s, sbyte delim)
	{
		List<string> elems = new List<string>();
		split(s, delim, elems);
		return elems;
	}

//C++ TO C# CONVERTER TODO TASK: The original C++ template specifier was replaced with a C# generic specifier, which may not produce the same behavior:
//ORIGINAL LINE: template <typename T>
	public static int vectorSearch<T>(List<T> vec, T value)
	{
		int pos = std.find(vec.GetEnumerator(), vec.end(), value) - vec.GetEnumerator();

//        if (pos >= vec.size())
//            return 0; // TODO: throw an error here
//        else
		return pos;
	}

	public static double regexFindDoubleTag(string input, string pattern)
	{
		std.regex rgx = new std.regex(pattern);
		std.smatch match = new std.smatch();

		if (!std.regex_search(input, match, rgx))
		{
			return 0.0; // TODO: throw error
		}

		return Convert.ToDouble((string)match[1]);
	}

	public static bool isCoordInRange(float x, float y, float z, float xs, float ys, float zs, float xf, float yf, float zf)
	{
		return (x >= xs != 0F && x <= xf != 0F && y >= ys != 0F && y <= yf != 0F && z >= zs != 0F && z <= zf);
	}


//    template <typename T>
//    static Matrix<T> generateNormalisedRotationMatrix(const Matrix<T>& A, const Matrix<T>& B)
//    {
//        Matrix<double> wm = Mat::Cross(A, B);

//        wm = wm / Mat::Norm(wm);

//        Matrix<double> w_hat({0.0, -wm(2), wm(1), wm(2), 0.0, -wm(0), -wm(1), wm(0), 0.0}, 3, 3);

//        double cos_tht = Mat::Dot(A, B) / Mat::Norm(A) / Mat::Norm(B);

//        double tht = std::acos(cos_tht);
//        Matrix<double> R = Mat::eye<double>(3);

//        // This is the rotation matrix we want
//        return R + w_hat * std::sin(tht) + Mat::Multiply(w_hat, w_hat) * (1 - std::cos(tht));
//    }

//    template <typename T>
//    static Matrix<T> generateRotationMatrix(Matrix<T> axis, double theta)
//    {
//        axis = axis / std::sqrt(Mat::Dot(axis, axis));
//        double a = std::cos(theta / 2);
//        auto bcd = -1*axis * std::sin(theta/2);
//        auto aa = a * a;
//        auto bb = bcd(0) * bcd(0);
//        auto cc = bcd(1) * bcd(1);
//        auto dd = bcd(2) * bcd(2);
//        auto ab = a * bcd(0);
//        auto ac = a * bcd(1);
//        auto ad = a * bcd(2);
//        auto bc = bcd(0) * bcd(1);
//        auto bd = bcd(0) * bcd(2);
//        auto cd = bcd(1) * bcd(2);

//        return Matrix<double>({aa+bb-cc-dd, 2*(bc+ad), 2*(bd-ac), 2*(bc-ad), aa+cc-bb-dd, 2*(cd+ab), 2*(bd+ac), 2*(cd-ab), aa+dd-bb-cc}, 3, 3);
//    }

//    template <typename T>
//    static void printMatrix(const Matrix<T>& input)
//    {
//        for(int j = 0; j < input.rows(); ++j)
//        {
//            for(int i = 0; i < input.cols(); ++i)
//            {
//                std::cout << input(j, i) << " ";
//            }
//            std::cout << std::endl;
//        }
//    }

}

