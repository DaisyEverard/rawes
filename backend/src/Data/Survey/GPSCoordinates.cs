
namespace MainApi.Data.Survey;


public class GPSCoordinates
{
    public double x { get; set; }
    public double y { get; set; }

    public GPSCoordinates(double new_x, double new_y) {
        x = new_x;
        y = new_y;
    }
}

