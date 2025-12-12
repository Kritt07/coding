using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Diagnostics;
using DynamicData.Tests;

namespace HotelAccounting;

class AccountingModel : ModelBase
{
    private double price;
    private int nightsCount;
    private double discount;
    private double total;

    public double Price
    {
        get => price;
        set
        {
            if (value < 0)
                throw new ArgumentException();

            price = value;
            total = price * nightsCount * (1 - discount / 100);

            Notify(nameof(Price));
            Notify(nameof(Total));
        }
    }

    public int NightsCount
    {
        get => nightsCount;
        set
        {
            if (value <= 0)
                throw new ArgumentException();

            nightsCount = value;
            total = price * nightsCount * (1 - discount / 100);

            Notify(nameof(NightsCount));
            Notify(nameof(Total));
        }
    }

    public double Discount
    {
        get => discount;
        set
        {
            discount = value;
            total = price * nightsCount * (1 - discount / 100);

            if (total < 0)
                throw new ArgumentException();
            
            Notify(nameof(Discount));
            Notify(nameof(Total));
        }
    }

    public double Total
    {
        get => total;
        set
        {
            if (value < 0)
                throw new ArgumentException();

            total = value;
            discount = 100 * (1 - total / (price * nightsCount));

            Notify(nameof(Total));
            Notify(nameof(Discount));
        }
    }
}
