//
// Created by Jon on 25/08/2015.
//

#ifndef XYZ_CELLGEOMETRY_H
#define XYZ_CELLGEOMETRY_H

#include <vector>

#include "Utilities.h"
#include <Eigen/Dense>

class CellGeometry
{
public:
    CellGeometry(){}
    CellGeometry(double ain, double bin, double cin, double alphain, double betain, double gammain);

    Eigen::Vector3d getAVector() { return avec; }
    Eigen::Vector3d getBVector() { return bvec; }
    Eigen::Vector3d getCVector() { return cvec; }

private:
    double a, b, c, alpha, beta, gamma;

    Eigen::Vector3d avec, bvec, cvec;

    void calculateCartesianBasis();
};


#endif //XYZ_CELLGEOMETRY_H
