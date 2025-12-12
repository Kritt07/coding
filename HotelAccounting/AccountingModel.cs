using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Diagnostics;

namespace HotelAccounting;

//создайте класс AccountingModel здесь

class AccountingModel : ModelBase
{
    private double price;
    private int nightsCount;
    private double discount;
    private double total;

    public double Price
    {
        get { return price; }
        set
        {
            if (value < 0) throw new ArgumentException();
            price = value;
            Notify(nameof(Price));
        }
    }

    public int NightsCount
    {
        get { return nightsCount; }
        set
        {
            if (value <= 0) throw new  ArgumentException();
            nightsCount = value;
            Notify(nameof(NightsCount));
        }
    }

    public double Discount
    { 
        get { return discount; }
        set
        {
            discount  = value;
            Notify(nameof(Discount));
        }
    }

    public double Total
    {
        get { return total; }
        set
        {
            if (value < 0) throw new ArgumentException();
            total = value;
            Notify(nameof(Total));
        }
    }

    private void Notify (string name)
    {
        if (name == nameof(Total))
        {
            var newDiscount = 100 * (1 - Total / (Price * NightsCount));
            Discount = newDiscount;
        } else 
            Total = Price * NightsCount * (1 - Discount / 100);
    }
}