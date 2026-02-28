using Hospital.Models;

namespace Hospital.ViewModels
{
    public class BedViewModel
    {
        public int Id { get; set; }
        public string BedNumber { get; set; }
        public int RoomId { get; set; }
        public bool IsOccupied { get; set; }

        public BedViewModel() { }

        public BedViewModel(Bed model)
        {
            Id = model.Id;
            BedNumber = model.BedNumber;
            RoomId = model.RoomId;
            IsOccupied = model.IsOccupied;
        }

        public Bed ConvertViewModel(BedViewModel model)
        {
            return new Bed
            {
                Id = model.Id,
                BedNumber = model.BedNumber,
                RoomId = model.RoomId,
                IsOccupied = model.IsOccupied
            };
        }
    }
}
