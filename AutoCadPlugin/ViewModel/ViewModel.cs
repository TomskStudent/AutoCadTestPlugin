using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

using AutoCadPlugin.Model;

using AcadDS = Autodesk.AutoCAD.DatabaseServices;
using AcadAS = Autodesk.AutoCAD.ApplicationServices;

namespace AutoCadPlugin.ViewModel
{
    /// <summary>
    /// Определение параметров объекта - набор слоев.
    /// </summary>
    public interface ILayers
    {
        ObservableCollection<Layer> Layers { get; set; }
        Layer SelectedLayer { get; set; }
    }

    /// <summary>
    /// Реализация класса набора слоев с уведомлением об изменении свойств.
    /// </summary>
    public class LayersViewModel : ILayers, INotifyPropertyChanged
    {
        #region PrivateFields
        private ObservableCollection<Layer> _layers;
        private Layer _selectedLayer;
        #endregion

        #region PublicFields
        public ObservableCollection<Layer> Layers
        {
            get { return _layers; }
            set
            {
                _layers = value;
                OnPropertyChanged("Layers");
            }
        }

        public Layer SelectedLayer
        {
            get { return _selectedLayer; }
            set
            {
                _selectedLayer = value;
                OnPropertyChanged("SelectedLayer");
            }
        }
        #endregion

        #region Constructors
        public LayersViewModel()
        {
            Load();
            OnPropertyChanged("Layers");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Перезагрузка параметров слоев документа.
        /// </summary>
        public void Reload()
        {
            Load();
            OnPropertyChanged("Layers");
        }

        /// <summary>
        /// Загружает в объект свойства всех слоев проекта с привязанными 
        /// к ним фигурами типа: точка, отрезок и окружность.
        /// </summary>
        private void Load ()
        {
            // Обнуляем коллекцию слоев.
            Layers = new ObservableCollection<Layer>();

            // Получаем текущий документ, доступ к командной строке и БД.
            Document doc = AcadAS.Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;

            // Начинаем транзакцию.
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // Получаем таблицу слоев документа.
                LayerTable lt = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);
                // Добавляем в нашу коллекцию все слои с требуемыми параметрами.
                foreach (ObjectId ltrId in lt)
                {
                    LayerTableRecord ltr = (LayerTableRecord)tr.GetObject(ltrId, OpenMode.ForRead);
                    Layers.Add(new Layer(ltr));
                }
                // Получаем список фигур (точка, отрезок и окружность) каждого слоя,
                // и сохраняем требуемыме параметры фигур в коллекции каждого слоя.
                foreach (var layer in Layers)
                {
                    // Обновляем предствление пользователя.
                    if (Layers[0] == layer)
                    {
                        _selectedLayer = layer;
                        OnPropertyChanged("SelectedLayer");
                    }                     

                    // Задание параметров фильтра.
                    TypedValue[] filterlist = new TypedValue[8];
                    filterlist[0] = new TypedValue((int)DxfCode.Operator, "<AND");
                    filterlist[1] = new TypedValue((int)DxfCode.LayerName, layer.Name);
                    filterlist[2] = new TypedValue((int)DxfCode.Operator, "<OR");
                    filterlist[3] = new TypedValue((int)DxfCode.Start, "POINT");
                    filterlist[4] = new TypedValue((int)DxfCode.Start, "LINE");
                    filterlist[5] = new TypedValue((int)DxfCode.Start, "CIRCLE");
                    filterlist[6] = new TypedValue((int)DxfCode.Operator, "OR>");
                    filterlist[7] = new TypedValue((int)DxfCode.Operator, "AND>");

                    // Создаем фильтр.
                    SelectionFilter filter = new SelectionFilter(filterlist);
                    // пытаемся получить ссылки на объекты с учетом фильтра
                    PromptSelectionResult selRes = ed.SelectAll(filter);

                    // Получаем массив ID объектов.
                    if (selRes.Value != null)
                    {
                        ObjectId[] ids = selRes.Value.GetObjectIds();

                        foreach (ObjectId id in ids)
                        {
                            // Приводим каждый из них к типу Entity.
                            Entity entity = (Entity)tr.GetObject(id, OpenMode.ForRead);
                            // Классифицируем по группам.
                            if (entity.GetType() == typeof(AcadDS.DBPoint))
                            {
                                layer.Points.Add(new Model.Point(entity as AcadDS.DBPoint));
                            }
                            else if (entity.GetType() == typeof(AcadDS.Line))
                            {
                                layer.Lines.Add(new Model.Line(entity as AcadDS.Line));
                            }
                            else if (entity.GetType() == typeof(AcadDS.Circle))
                            {
                                layer.Circles.Add(new Model.Circle(entity as AcadDS.Circle));
                            }
                        }
                    }
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// Загружает в документ новые параметры слоев и фигур.
        /// </summary>
        public void Update()
        {
            // Получаем текущий документ, доступ к командной строке и БД.
            Document doc = AcadAS.Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;           

            // Блокируем документ для редактирования из вне.
            using (DocumentLock lc = doc.LockDocument())
            {
                // Начинаем транзакцию.
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        // Создаем список имен слоев.
                        string[] layerNames = new string[Layers.Count];
                        int i = 0;
                        foreach (Layer layer in Layers)
                        {
                            layerNames[i] = layer.Name;
                            i++;
                        }

                        i = 0;
                        // Получаем таблицу слоев.
                        LayerTable lt = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForWrite);
                        // Задаем временные имена слоев для избежания конфликтов переименовывания.
                        foreach (ObjectId ltrId in lt)
                        {
                            LayerTableRecord ltr = (LayerTableRecord)tr.GetObject(ltrId, OpenMode.ForWrite);
                            if (ltr.Name != "0")
                            {
                                ltr.Name = "_temp_" + ltr.Name + "_temp_";
                            }
                        }

                        foreach (Layer layer in Layers)
                        {
                            // Проверяем новое имя слоя на дублирование и присваиваем слоям документа новые параметры.
                            if (Array.IndexOf(layerNames, layer.Name) == Array.LastIndexOf(layerNames, layer.Name))
                            {
                                LayerTableRecord acadLayer = (LayerTableRecord)tr.GetObject(layer.Id, OpenMode.ForWrite);
                                layer.Update(ref acadLayer);
                            }      
                            else if (Array.IndexOf(layerNames, layer.Name) == i)
                            {
                                MessageBox.Show("Дублирование имени слоя \"" + layer.Name + "\".");
                            }
                            i++;

                            // Присваиваем фигурам документа новые параметры.
                            foreach (Model.Point point in layer.Points)
                            {
                                AcadDS.DBPoint acadPoint = (AcadDS.DBPoint)tr.GetObject(point.Id, OpenMode.ForWrite);
                                point.Update(ref acadPoint);
                            }

                            foreach (Model.Line line in layer.Lines)
                            {
                                AcadDS.Line acadLine = (AcadDS.Line)tr.GetObject(line.Id, OpenMode.ForWrite);
                                line.Update(ref acadLine);
                            }

                            foreach (Model.Circle circle in layer.Circles)
                            {
                                AcadDS.Circle acadCircle = (AcadDS.Circle)tr.GetObject(circle.Id, OpenMode.ForWrite);
                                circle.Update(ref acadCircle);
                            }
                        }

                        // Если не все слои переименовались удаляем добавочный преффик и суффикс.       
                        foreach (ObjectId ltrId in lt)
                        {
                            LayerTableRecord ltr = (LayerTableRecord)tr.GetObject(ltrId, OpenMode.ForWrite);
                            ltr.Name = ltr.Name.Replace("_temp_", "");
                        }
                    }
                    catch (Autodesk.AutoCAD.Runtime.Exception e)
                    {
                        ed.WriteMessage("Ошибка применения данных. {0}: {1}", e.HResult, e.Message);
                    }
                    tr.Commit();
                    tr.Dispose();
                }
                //lc.Dispose();
            }
            // Обновляем экземпляр класса после изменения документа.
            Reload();            
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

