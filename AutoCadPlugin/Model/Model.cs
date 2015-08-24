using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;
using SysWM = System.Windows.Media;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

using AcadC = Autodesk.AutoCAD.Colors;
using AcadDS = Autodesk.AutoCAD.DatabaseServices;
using AcadAS = Autodesk.AutoCAD.ApplicationServices;


namespace AutoCadPlugin.Model
{
    /// <summary>
    /// Определение плоской системы координат.
    /// </summary>
    public interface ICoordinates
    {
        double X { get; set; }
        double Y { get; set; }
    }

    /// <summary>
    /// Класс плоской системы координат с уведомлением пользователя об изменении свойств.
    /// </summary>
    public class Coordinates : ICoordinates, INotifyPropertyChanged
    {
        #region PrivateFields
        private double _x;
        private double _y;
        #endregion

        #region PublicFields
        public double X
        {
            get { return _x; }
            set
            {
                _x = value;
                OnPropertyChanged("X");
            }
        }

        public double Y
        {
            get { return _y; }
            set
            {
                _y = value;
                OnPropertyChanged("Y");
            }
        }
        #endregion

        #region PropertyChange
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    /// <summary>
    /// Определение параметров объекта - точки.
    /// </summary>
    public interface IPoint
    {
        ObjectId Id { get; set; }
        string Name { get; set; }
        Coordinates Position { get; set; }
        double Height { get; set; }
    }

    /// <summary>
    /// Реализация класса точки с уведомлением об изменении свойств.
    /// </summary>
    public class Point : IPoint, INotifyPropertyChanged
    {
        #region PrivateFields
        private ObjectId _id;
        private string _name;
        private Coordinates _position;
        private double _height;
        #endregion

        #region PublicFields
        public Point ()
        {
            _id = new ObjectId();
            _name = "point";
            _position = new Coordinates() { X = 0, Y = 0 };
            _height = 0;
        }

        public Point (DBPoint point)
        {
            _id = point.Id;
            _name = "point";
            _position = new Coordinates() { X = point.Position.X, Y = point.Position.Y };
            _height = point.Position.Z;
        }
             
        public ObjectId Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public Coordinates Position
        {
            get { return _position; }
            set
            {
                _position = value;
                OnPropertyChanged("Position");
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged("Height");
            }
        }
        #endregion

        /// <summary>
        /// Задание параметров точки класса DBPoint.
        /// </summary>
        /// <param name="acadPoint">Редактируемая точка</param>
        public void Update (ref AcadDS.DBPoint acadPoint)
        {
            if (acadPoint != null)
            {
                acadPoint.Position = new Point3d(Position.X, Position.Y, Height);
            }            
        }

        #region PropertyChange
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    /// <summary>
    /// Определение параметров объекта - отрезка.
    /// </summary>
    public interface ILine
    {
        ObjectId Id { get; set; }
        string Name { get; set; }
        Coordinates StartPoint { get; set; }
        Coordinates EndPoint { get; set; }
        double Height { get; set; }
    }

    /// <summary>
    /// Реализация класса отрезка с уведомлением об изменении свойств.
    /// </summary>
    public class Line : ILine, INotifyPropertyChanged
    {
        #region PrivateFields
        private ObjectId _id;
        private string _name;
        private Coordinates _startPoint;
        private Coordinates _endPoint;
        private double _height;
        #endregion

        #region Constructors
        public Line()
        {
            _id = new ObjectId();
            _name = "line";
            _startPoint = new Coordinates() { X = 0, Y = 0 };
            _endPoint = new Coordinates() { X = 0, Y = 0 };
            _height = 0;
        }

        public Line (AcadDS.Line line)
        {
            _id = line.Id;
            _name = "line";
            _startPoint = new Coordinates() { X = line.StartPoint.X, Y = line.StartPoint.Y };
            _endPoint = new Coordinates() { X = line.EndPoint.X, Y = line.EndPoint.Y };
            _height = line.StartPoint.Z;
        }
        #endregion

        #region PublicFields
        public ObjectId Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public Coordinates StartPoint
        {
            get { return _startPoint; }
            set
            {
                _startPoint = value;
                OnPropertyChanged("StartPoint");
            }
        }

        public Coordinates EndPoint
        {
            get { return _endPoint; }
            set
            {
                _endPoint = value;
                OnPropertyChanged("EndPoint");
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged("Height");
            }
        }
        #endregion

        /// <summary>
        /// Задание параметров отрезка класса Line.
        /// </summary>
        /// <param name="acadLine">Редактируемый отрезок</param>
        public void Update(ref AcadDS.Line acadLine)
        {
            if (acadLine != null)
            {
                acadLine.StartPoint = new Point3d(StartPoint.X, StartPoint.Y, Height);
                acadLine.EndPoint = new Point3d(EndPoint.X, EndPoint.Y, Height);
            }            
        }

        #region PropertyChange
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    /// <summary>
    /// Определение параметров объекта - окружности.
    /// </summary>
    public interface ICircle
    {
        ObjectId Id { get; set; }
        string Name { get; set; }
        Coordinates Center { get; set; }
        double Radius { get; set; }
        double Height { get; set; }
    }

    /// <summary>
    /// Реализация класса окружность с уведомлением об изменении свойств.
    /// </summary>
    public class Circle : ICircle, INotifyPropertyChanged
    {
        #region PrivateFields
        private ObjectId _id;
        private string _name;
        private Coordinates _center;
        private double _radius;
        private double _height;
        #endregion

        #region Constructors
        public Circle ()
        {
            _id = new ObjectId();
            _name = "circle";
            _center = new Coordinates() { X = 0, Y = 0 };
            _height = 0;
        }

        public Circle (AcadDS.Circle circle)
        {
            _id = circle.Id;
            _name = "circle";
            _center = new Coordinates() { X = circle.Center.X, Y = circle.Center.Y };
            _radius = circle.Radius;
            _height = circle.Center.Z;
        }
        #endregion

        #region PublicFields
        public ObjectId Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public Coordinates Center
        {
            get { return _center; }
            set
            {
                _center = value;
                OnPropertyChanged("Center");
            }
        }

        public double Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                OnPropertyChanged("Radius");
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged("Height");
            }
        }
        #endregion

        /// <summary>
        /// Задание параметров окружности класса Circle.
        /// </summary>
        /// <param name="acadCircle">Редактируемая окружность</param>
        public void Update(ref AcadDS.Circle acadCircle)
        {  
            if (acadCircle != null)
            {
                acadCircle.Center = new Point3d(Center.X, Center.Y, Height);
                acadCircle.Radius = Radius;
            }            
        }

        #region PropertyChange
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    /// <summary>
    /// Определение свойств слоя.
    /// </summary>
    public interface ILayer
    {
        ObjectId Id { get; set; }
        string Name { get; set; }
        AcadC.Color Color { get; set; }
        bool Visibility { get; set; }
        ObservableCollection<Point> Points { get; set;}
        ObservableCollection<Line> Lines { get; set; }
        ObservableCollection<Circle> Circles { get; set; }
        Point SelectedPoint { get; set; }
        Line SelectedLine { get; set; }
        Circle SelectedCircle { get; set; }
    }

    /// <summary>
    /// Реализация класса слой с уведомлением об изменении свойств.
    /// </summary>
    public class Layer : ILayer, INotifyPropertyChanged
    {
        #region PrivateFields
        private ObjectId _id;
        private string _name;
        AcadC.Color _color;
        private bool _visibility;
        private ObservableCollection<Point> _points;
        private ObservableCollection<Line> _lines;
        private ObservableCollection<Circle> _circles;
        private Point _selectedPoint;
        private Line _selectedLine;
        private Circle _selectedCirle;
        #endregion

        #region Constructors
        public Layer ()
        {
            _id = new ObjectId();
            _name = "layer";
            _color = AcadC.Color.FromRgb(255, 255, 255);
            _visibility = true;

            _points = new ObservableCollection<Point>();
            _lines = new ObservableCollection<Line>();
            _circles = new ObservableCollection<Circle>();

            _selectedPoint = new Point();
            _selectedLine = new Line();
            _selectedCirle = new Circle();
        }

        public Layer (LayerTableRecord layer)
        {
            _id = layer.Id;
            _name = layer.Name;
            _color = layer.Color;
            _visibility = !layer.IsOff;

            _points = new ObservableCollection<Point>();
            _lines = new ObservableCollection<Line>();
            _circles = new ObservableCollection<Circle>();

            _selectedPoint = new Point();
            _selectedLine = new Line();
            _selectedCirle = new Circle();            
        }
        #endregion

        #region PublicFields
        public ObjectId Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != "0")
                {
                    if (IsBadName(value))
                    {
                        MessageBox.Show("Имя некорректно.");
                    }
                    else
                    {
                        _name = value;
                    }                    
                }                    
                else
                {
                    MessageBox.Show("Слой 0 нельзя переименовать.");
                }
                OnPropertyChanged("Name");
            }
        }

        public AcadC.Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                OnPropertyChanged("Color");
                OnPropertyChanged("Brush");
            }
        }

        public SysWM.SolidColorBrush Brush
        {
            get { return DefineColorBrush(); }
        }

        public bool Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                OnPropertyChanged("Visibility");
            }
        }

        public ObservableCollection<Point> Points
        {
            get { return _points; }
            set
            {
                _points = value;                

                if (_points.Count >0 )
                    foreach (Point point in _points)
                    {
                        SelectedPoint = point;
                        break;
                    }

                OnPropertyChanged("Points");
            }
        }

        public ObservableCollection<Line> Lines
        {
            get { return _lines; }
            set
            {
                _lines = value;

                if (_lines.Count > 0)
                    foreach (Line line in _lines)
                    {
                        SelectedLine = line;
                        break;
                    }

                OnPropertyChanged("Lines");
            }
        }

        public ObservableCollection<Circle> Circles
        {
            get { return _circles; }
            set
            {
                _circles = value;

                if(_circles.Count > 0)
                    foreach (Circle circle in _circles)
                    {
                        SelectedCircle = circle;
                        break;
                    }

                OnPropertyChanged("Circles");
            }
        }

        public Point SelectedPoint
        {
            get { return _selectedPoint; }
            set
            {
                _selectedPoint = value;
                OnPropertyChanged("SelectedPoint");
            }
        }

        public Line SelectedLine
        {
            get { return _selectedLine; }
            set
            {
                _selectedLine = value;
                OnPropertyChanged("SelectedLine");
            }
        }

        public Circle SelectedCircle
        {
            get { return _selectedCirle; }
            set
            {
                _selectedCirle = value;
                OnPropertyChanged("SelectedCircle");
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Переопределение параметров задаваемого слоя согласно свойствам объекта класса.
        /// </summary>
        /// <param name="acadLayer"></param>
        public void Update (ref LayerTableRecord acadLayer)
        {
            if (acadLayer != null)
            {
                if (acadLayer.Name != "0")
                {
                    if (!acadLayer.Name.Contains("|"))
                    {
                        acadLayer.Name = Name;
                    }
                    else
                    {
                        MessageBox.Show("Слой не может быть переименован, т.к. содержит внешние ссылки.");
                    }
                }
                else if (acadLayer.Name != Name)
                {
                    MessageBox.Show("Слой 0 нельзя переименовать.");
                }
                    
                acadLayer.IsOff = !Visibility;
                acadLayer.Color = Color;
            }            
        }

        /// <summary>
        /// Открытие диалогового окна цветовой палитры для изменения свойства Color объекта.
        /// </summary>
        public void ChangeColorByDlg()
        {
            AcadAS.Document doc = AcadAS.Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            Autodesk.AutoCAD.Windows.ColorDialog dlg = new Autodesk.AutoCAD.Windows.ColorDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Color = dlg.Color;
            }
        }

        /// <summary>
        /// Переопределение цвета объекта в цветовую палитру Windows.
        /// </summary>
        /// <returns></returns>
        private SysWM.SolidColorBrush DefineColorBrush()
        {
            SysWM.Color sysColor = SysWM.Color.FromRgb(Color.Red, Color.Green, Color.Blue);

            if (!Color.IsByAci)
            {
                if ((Color.IsByLayer) || (Color.IsByBlock))
                {
                    sysColor = SysWM.Color.FromRgb((byte)255, (byte)255, (byte)255);
                }                
                else
                {
                    sysColor = SysWM.Color.FromRgb(Color.Red, Color.Green, Color.Blue);
                }
            }
            else
            {
                short colIndex = Color.ColorIndex;

                System.Byte byt = System.Convert.ToByte(colIndex);
                int rgb = Autodesk.AutoCAD.Colors.EntityColor.LookUpRgb(byt);
                long b = (rgb & 0xffL);
                long g = (rgb & 0xff00L) >> 8;
                long r = rgb >> 16;

                if (colIndex == 7)
                {
                    if (r <= 128 && g <= 128 && b <= 128)
                    {// White
                        sysColor = SysWM.Color.FromRgb((byte)255, (byte)255, (byte)255);
                    }
                    else
                    {// Black
                        sysColor = SysWM.Color.FromRgb((byte)0, (byte)0, (byte)0);
                    }
                }
                else
                {
                    sysColor = SysWM.Color.FromRgb((byte)r, (byte)g, (byte)b);
                }
            }

            return new SysWM.SolidColorBrush(sysColor);
        }

        /// <summary>
        /// Проверка на задание некорректного имени слоя.
        /// </summary>
        /// <param name="name">Имя слоя.</param>
        /// <returns></returns>
        private bool IsBadName(string name)
        {
            name = name.Replace(" ", "");
            if (name.Length > 0)
            {
                var badChars = "<>/\'\":;?,*|=";
                return name.Intersect(badChars).Any();
            }
            return true;            
        }
        #endregion

        #region PropertyChange
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}