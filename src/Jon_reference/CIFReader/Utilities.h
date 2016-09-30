//
// Created by Jon on 23/08/2015.
//

#ifndef XYZ_UTILITIES_H
#define XYZ_UTILITIES_H

#include <string>
#include <vector>
#include <sstream>

#include <regex>

#include <cassert>
#include <iostream>
#include <Eigen/Dense>

const double PI = 2*acos(0.0);

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

static struct Utilities
{
    static std::vector<std::string> &split(const std::string &s, char delim, std::vector<std::string> &elems) {
        std::stringstream ss(s);
        std::string item;
        while (std::getline(ss, item, delim)) {
            elems.push_back(item);
        }
        return elems;
    }


    static std::vector<std::string> split(const std::string &s, char delim) {
        std::vector<std::string> elems;
        split(s, delim, elems);
        return elems;
    }

    template <typename T>
    static int vectorSearch(std::vector<T> vec, T value)
    {
        int pos = std::find(vec.begin(), vec.end(), value) - vec.begin();

//        if (pos >= vec.size())
//            return 0; // TODO: throw an error here
//        else
        return pos;
    }

    static double regexFindDoubleTag(std::string input, std::string pattern)
    {
        std::regex rgx(pattern);
        std::smatch match;

        if (!std::regex_search(input, match, rgx))
            return 0.0; // TODO: throw error

        return std::stod(std::string(match[1]));
    }

    static bool isCoordInRange(float x, float y, float z, float xs, float ys, float zs, float xf, float yf, float zf)
    {
        return ( x >= xs && x <= xf && y >= ys && y <= yf && z >= zs && z <= zf );
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

} Utilities;

#endif //XYZ_UTILITIES_H
