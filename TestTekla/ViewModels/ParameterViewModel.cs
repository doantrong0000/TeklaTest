using System;
using Tekla.Structures.Model;
using TeklaApp.Models;

namespace TeklaApp.ViewModels
{
    public class ParameterViewModel
    {
        private TeklaModelMng _teklaModel;

        public ParameterViewModel()
        {
            _teklaModel = new TeklaModelMng();
        }

        public string ReadParameters()
        {
            if (!_teklaModel.IsConnected())
            {
                return "Lỗi: Không tìm thấy Tekla Structures đang mở.";
            }

            try
            {
                Tekla.Structures.Model.UI.Picker picker = new Tekla.Structures.Model.UI.Picker();
                ModelObject pickedObject = picker.PickObject(Tekla.Structures.Model.UI.Picker.PickObjectEnum.PICK_ONE_OBJECT, "Vui lòng chọn đối tượng để đọc thông số");

                if (pickedObject is Part part)
                {
                    string info = "Thông số User Defined Attributes (UDA):\r\n\r\n";
                    info += $"- Tên cấu kiện: {part.Name}\r\n";

                    string userField1 = "";
                    part.GetUserProperty("USER_FIELD_1", ref userField1);
                    info += $"- User field 1: {userField1}\r\n";

                    string userField2 = "";
                    part.GetUserProperty("USER_FIELD_2", ref userField2);
                    info += $"- User field 2: {userField2}\r\n";

                    string userField3 = "";
                    part.GetUserProperty("USER_FIELD_3", ref userField3);
                    info += $"- User field 3: {userField3}\r\n";

                    string userField4 = "";
                    part.GetUserProperty("USER_FIELD_4", ref userField4);
                    info += $"- User field 4: {userField4}\r\n";

                    string comment = "";
                    part.GetUserProperty("comment", ref comment);
                    info += $"- Comment: {comment}\r\n";

                    string prelimMark = "";
                    part.GetUserProperty("PRELIM_MARK", ref prelimMark);
                    info += $"- Preliminary mark: {prelimMark}\r\n";

                    return info;
                }
                else
                {
                    return "Đối tượng được chọn không phải là Part. Loại: " + (pickedObject != null ? pickedObject.GetType().Name : "null");
                }
            }
            catch (Exception ex)
            {
                return "Đã hủy chọn hoặc có lỗi: " + ex.Message;
            }
        }
    }
}
