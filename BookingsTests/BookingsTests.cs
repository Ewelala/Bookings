using System;
using System.Collections.Generic;
using System.Linq;
using Bookings;
using Moq;
using NUnit.Framework;
namespace BookingsTests
{
    [TestFixture]
    public class BookingsTests
    {

        private Booking _booking;
        private Mock<IBookingRepository> _mock;

        [SetUp]
        public void SetUp()
        {
            _booking = new Booking
            {
                Id = 1,
                ArrivalDate = DateTime.Today,
                DepartureDate = DateTime.Today.AddDays(7),
                Reference = "reference",
                Status = "Zarezerwowane"
            };

            _mock = new Mock<IBookingRepository>();
            _mock.Setup(r => r.GetActiveBookings(1)).Returns(new List<Booking>
            {
              _booking
            }.AsQueryable());
            _mock.Setup(r => r.GetActiveBookings(2)).Returns(new List<Booking>
            {
              _booking
            }.AsQueryable());
        }

        [Test]
        public void StartEndsBefore_ReturnEmptyString()
        {
            var result = BookingHelper.OverlappingBookingsExist(new Booking
            {
                Id = 2,
                ArrivalDate = Before(_booking.ArrivalDate, days: 5),
                DepartureDate = Before(_booking.ArrivalDate)
            },
            _mock.Object);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void StartsBeforeAndEndsIn_ReturnNotEmpty()
        {
            var result = BookingHelper.OverlappingBookingsExist(new Booking
            {
                Id = 2,
                ArrivalDate = Before(_booking.ArrivalDate, days: 2),
                DepartureDate = Before(_booking.DepartureDate)
            },
            _mock.Object);
            Assert.AreEqual("reference", result);
        }

        [Test]
        public void StartsAndEndsIn_ReturnNotEmpty()
        {
            var result = BookingHelper.OverlappingBookingsExist(new Booking
            {
                Id = 2,
                ArrivalDate = _booking.ArrivalDate,
                DepartureDate = _booking.DepartureDate
            },
            _mock.Object);
            Assert.AreEqual("reference", result);
        }

        [Test]
        public void StartsInEndsAfter_ReturnNotEmpty()
        {
            var result = BookingHelper.OverlappingBookingsExist(new Booking
            {
                Id = 2,
                ArrivalDate = _booking.ArrivalDate,
                DepartureDate = After(_booking.DepartureDate, 2)
            },
            _mock.Object);
            Assert.AreEqual("reference", result);
        }

        [Test]
        public void StartsEndsAfter_ReturnEmptyString()
        {
            var result = BookingHelper.OverlappingBookingsExist(new Booking
            {
                Id = 2,
                ArrivalDate = After(_booking.DepartureDate),
                DepartureDate = After(_booking.DepartureDate, 2)
            },
            _mock.Object);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void StartsBeforeEndsAfter_ReturnNotEmpty()
        {
            var result = BookingHelper.OverlappingBookingsExist(new Booking
            {
                Id = 2,
                ArrivalDate = Before(_booking.ArrivalDate),
                DepartureDate = After(_booking.DepartureDate)
            },
            _mock.Object);
            Assert.AreEqual("reference", result);
        }

        private DateTime ArivalOn(int year, int month, int day)
        {
            return new DateTime(year, month, day, 14, 0, 0);
        }

        private DateTime Before(DateTime arrivalDate, int days = 1)
        {
            return arrivalDate.AddDays(-days);
        }

        private DateTime After(DateTime dateTime, int days = 1)
        {
            return dateTime.AddDays(days);
        }

        private DateTime DepartOn(int year, int month, int day)
        {
            return new DateTime(year, month, day, 10, 0, 0);
        }
    }
}
