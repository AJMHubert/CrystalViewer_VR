//
// Created by Jon on 21/08/2015.
//

#include "CIFReader.h"

#include <iostream>

CIFReader::CIFReader(std::string filePath)
{
    // http://stackoverflow.com/questions/2602013/read-whole-ascii-file-into-c-stdstring
    std::ifstream filestream(filePath);
    std::string filecontents;

    filestream.seekg(0, std::ios::end);
    filecontents.reserve(filestream.tellg());
    filestream.seekg(0, std::ios::beg);

    filecontents.assign((std::istreambuf_iterator<char>(filestream)), std::istreambuf_iterator<char>());

    // read in the symmetry elements
    readSymmetryOperations(filecontents);

    // read in atom positions
    readAtomPositions(filecontents);

    // read in unit cell parameters
    readCellGeometry(filecontents);
}

void CIFReader::readSymmetryOperations(const std::string &input)
{
    std::vector<Symmetry> symmetrylist_temp;

    // first we want to know how  columns we have and where the things we want are
    std::regex rgxheaders("(?:loop_\\n)((?:_symmetry_equiv_pos_\\w+?\\n)+)");
    std::smatch match;

    if (!std::regex_search(input, match, rgxheaders))
        throw std::runtime_error("Cannot find _symmetry_equiv_pos_ block.");

    std::vector<std::string> headerlines = Utilities::split(match[1], '\n');

    int xyzcol = Utilities::vectorSearch(headerlines, std::string("_symmetry_equiv_pos_as_xyz"));

    if (xyzcol >= headerlines.size())
        throw std::runtime_error("Could not find _symmetry_equiv_pos_as_xyz.");

    // now extract the block after the headers. This needs to be done dynamically depending on the headers
    std::string symmetrypattern = "(?:loop_\\n)(?:_symmetry_equiv_pos_\\w+?\\n)+((?:";

    // this is a bit more tricky as all the columns don't have the same regex
    for (int i = 0; i < headerlines.size(); ++i)
    {
        if (i == xyzcol)
            symmetrypattern += "(?:['\"]?.+?,.+?,.+?['\"]?)";
        else
            symmetrypattern += "\\S+";
        if (i != headerlines.size()-1)
            symmetrypattern += "[ \\t]+";
    }

    symmetrypattern += "\\n)+)";

    std::regex rgxsymmetry(symmetrypattern);
    if (!std::regex_search(input, match, rgxsymmetry))
        throw std::runtime_error("Problem parsing _symmetry_equiv_pos_ block.");

    // split the lines
    std::vector<std::string> symmetrylines = Utilities::split(match[1], '\n');

    // now precess each line to get the symmetry
    for (std::string symmetry : symmetrylines)
    {
        std::regex rgxsymmetry("(['\"].+?['\"]|[\\S]+)");

        std::vector<std::string> column;

        while (std::regex_search(symmetry, match, rgxsymmetry))
        {
            column.push_back(match[1]);
            symmetry = match.suffix().str();
        }

        std::string symmetryxyz = column[xyzcol];

        // this first regex splits the lines into the individual operations
        std::regex rgxxyz("['\"]*\\s*([^\\s'\"]+)\\s*,\\s*([^\\s'\"]+)\\s*,\\s*([^\\s'\"]+)\\s*['\"]*");

        if (!std::regex_search(symmetryxyz, match, rgxxyz))
            throw std::runtime_error("Problem parsing _symmetry_equiv_pos_ line.");

        auto matched = match;
        Symmetry ops;

        // loop through each operation ( we know there will be 3)
        for(int i = 1; i < 4; ++i)
        {
            // the operation string
            std::string op = matched[i];

            std::regex rgxoperation("([+-]?[^+-]+)");

//            if (!std::regex_search(op, match, rgxoperation))
//                throw std::runtime_error("Problem parsing symmetry operation.");

            std::vector<std::string> termstrings;

//            for (int i = 1; i < match.size(); ++i)
//            {
//                termstrings.push_back(match[i]);
//            }
//            std::regex rgxoperation("([\\+\\-/\\d]*?)([\\+\\-][xyz])([\\+\\-]\\d+/*\\d*){0,1}");

//            // TODO: maybe check that we have one match first so we can throw an error?
//            // TODO: check we are reading in 3 operations for each line?

//            // TODO: preallocate this vector
//            std::vector<std::vector<std::string>> termstrings;

//            // match all individual terms in the one operation
//            // use a while loop now as we expect more than one match per string
            while (std::regex_search(op, match, rgxoperation))
            {
//                std::vector<std::string> o = {"", "", ""};

//                for (int j = 1; j < match.size(); ++j)
//                    o[j - 1] = match[j];

                for (int i = 1; i < match.size(); ++i)
                {
                    termstrings.push_back(match[i]);
                }

//                // append to our vector of terms
//                termstrings.push_back(o);

//                // trim the string to make sure we don't find the same match
                op = match.suffix().str();
            }

            if (termstrings.size() < 1)
                throw std::runtime_error("Problem parsing symmetry operation.");

            // split terms by +-

            ops.setOperation(i-1, termstrings);
        }

        // append ops to vector to return
        symmetrylist_temp.push_back(ops);
    }

    symmetrylist = symmetrylist_temp;
}

void CIFReader::readAtomPositions(const std::string &input)
{
    // first we need to know where the positions we need are kept (column wise)
    // we do this through the header declaration just after the "loop_"
    std::regex rgxheaders("(?:loop_\\n)((?:_atom_site_\\w+?\\n)+)");
    std::smatch match;

    if (!std::regex_search(input, match, rgxheaders))
        throw std::runtime_error("Cannot find _atom_site_ block.");

    std::vector<std::string> headerlines = Utilities::split(match[1], '\n');

    // now we can find the values we want and this will be the columns numbers
    // TODO: error handling
    int xcol = Utilities::vectorSearch(headerlines, std::string("_atom_site_fract_x"));
    int ycol = Utilities::vectorSearch(headerlines, std::string("_atom_site_fract_y"));
    int zcol = Utilities::vectorSearch(headerlines, std::string("_atom_site_fract_z"));
    int symbolcol = Utilities::vectorSearch(headerlines, std::string("_atom_site_type_symbol"));   
    // this is not absolutely needed (assume occupancy = 1 otherwise
    int occupancycol = Utilities::vectorSearch(headerlines, std::string("_atom_site_occupancy"));

    if (xcol >= headerlines.size())
        throw std::runtime_error("Could not find _atom_site_fract_x.");

    if (ycol >= headerlines.size())
        throw std::runtime_error("Could not find _atom_site_fract_y.");

    if (zcol >= headerlines.size())
        throw std::runtime_error("Could not find _atom_site_fract_z.");

    if (symbolcol >= headerlines.size())
        throw std::runtime_error("Could not find _atom_site_type_symbol.");

    // now search for the lines after the headers
    // construct the regex dynamically (we don't know the number of columns beforehand_
    std::string positionpattern = "(?:loop_\\n)(?:_atom_site_\\w+?\\n)+((?:\\s*";
    for (int i = 0; i < headerlines.size()-1; ++i)
        positionpattern += "\\S+[ \\t]+";
    // the last column needs to be different to close off the regex
    positionpattern += "\\S+[ \\t]*\\n)+)";

    std::regex rgxpositions(positionpattern);
    if (!std::regex_search(input, match, rgxpositions))
        throw std::runtime_error("Problem parsing _atom_site_ block.");

    // split the lines
    std::vector<std::string> atomlines = Utilities::split(match[1], '\n');

    std::vector<AtomSite> atomsites_temp;

    for (auto line : atomlines)
    {
        // split each line by whitespace (use a regex for this?)
        std::regex rgxcolumns("([^\\s]+)");
        std::vector<std::string> columns;


        // TODO: this loop is infinite?
        while (std::regex_search(line, match, rgxcolumns))
        {
            // extract column into list of vectors
            for (int j = 1; j < match.size(); ++j)
                columns.push_back(match[j]);

            line = match.suffix().str();
        }

        // using the column indices from before, extract the required values
        // will need to remove brackets with errors in
        double x = std::stod(Utilities::split(columns[xcol], '(')[0]);
        double y = std::stod(Utilities::split(columns[ycol], '(')[0]);
        double z = std::stod(Utilities::split(columns[zcol], '(')[0]);
        // name has to be trimmed to remove oxidation state etc..
        std::string symbol = "";
        std::regex rgxname("([a-zA-Z]{1,2})");
        if (std::regex_search(columns[symbolcol], match, rgxname))
            symbol = match[1];
        // TODO: add handling for when occupancy column does not exist
        double occupancy = std::stod(Utilities::split(columns[occupancycol], '(')[0]);

        // here we are checking if the atom is on the same site
        Eigen::Vector3d postemp(x, y, z);
        bool isNew = true;

        for (auto& site : atomsites_temp)
        {
            auto positions = site.getPositions();
            int ind = Utilities::vectorSearch(positions, postemp);
            if (ind >= positions.size() )
                continue;
            else
            {
                site.addAtom(symbol, occupancy);
                isNew = false;
                //TODO: think i can break the loop here
            }
        }

        if (isNew)
            atomsites_temp.push_back(AtomSite(symmetrylist, symbol, x, y, z, occupancy));
    }

    // form new class instance from these and add to vector

    atomsites = atomsites_temp;
}

void CIFReader::readCellGeometry(const std::string &input)
{
    double a = Utilities::regexFindDoubleTag(input, "_cell_length_a\\s+([\\d.]+)");
    double b = Utilities::regexFindDoubleTag(input, "_cell_length_b\\s+([\\d.]+)");
    double c = Utilities::regexFindDoubleTag(input, "_cell_length_c\\s+([\\d.]+)");

    double alpha = Utilities::regexFindDoubleTag(input, "_cell_angle_alpha\\s+([\\d.]+)");
    double beta = Utilities::regexFindDoubleTag(input, "_cell_angle_beta\\s+([\\d.]+)");
    double gamma = Utilities::regexFindDoubleTag(input, "_cell_angle_gamma\\s+([\\d.]+)");

    cell = CellGeometry(a, b, c, alpha, beta, gamma);
}
