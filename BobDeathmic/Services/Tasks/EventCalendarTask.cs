﻿using BobDeathmic.Args;
using BobDeathmic.Data;
using BobDeathmic.Eventbus;
using BobDeathmic.Helper;
using BobDeathmic.Models.Events;
using BobDeathmic.Services.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BobDeathmic.Services.Tasks
{
    public class EventCalendarTask : IScheduledTask
    {
        public string Schedule => "0 18 * * *";
        private readonly ApplicationDbContext _context;
        private IEventBus _eventBus;
        private IConfiguration _configuration;
        public EventCalendarTask(ApplicationDbContext context, IEventBus eventBus, IConfiguration Configuration)
        {
            _context = context;
            _eventBus = eventBus;
            _configuration = Configuration;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if(_context != null)
            {
                var ApplicableCalendars = _context.Events.Include(x => x.EventDateTemplates).Include(x => x.EventDates).ThenInclude(x => x.Teilnahmen).Include(x => x.Members).ThenInclude(x => x.ChatUserModel).Where(x => x.Members.Count() > 0 && x.EventDateTemplates.Count() > 0);
                RemovePassedEventDates();
                foreach (Event calendar in ApplicableCalendars)
                {
                    AddEventDatesOnCalendar(calendar);
                    UpdateEventDates(calendar);
                    _context.SaveChanges();
                    NotifyUsers(calendar);
                    NotifyAdmin(calendar);
                }
            }
        }

        private void NotifyAdmin(Event calendar)
        {
            var NotifiableEventDates = calendar.EventDates.Where(x => x.Teilnahmen.Where(y => y.State == AppointmentRequestState.Available || y.State == AppointmentRequestState.IfNeedBe).Count() == x.Teilnahmen.Count());
            foreach (EventDate date in NotifiableEventDates)
            {
                _eventBus.TriggerEvent(EventType.DiscordWhisperRequested, new DiscordWhisperArgs { UserName = calendar.Admin.ChatUserName, Message = date.AdminNotification(calendar.Name) });
            }
        }

        private void NotifyUsers(Event calendar)
        {
            Console.WriteLine("NotifyUsers");
            string address = _configuration.GetValue<string>("WebServerWebAddress");
            List<MutableTuple<string, string>> GroupedNotifications = new List<MutableTuple<string, string>>();
            foreach (EventDate date in calendar.EventDates.Where(x => x.Date.Date == DateTime.Now.Add(TimeSpan.FromDays(5)).Date || x.Date.Date == DateTime.Now.Add(TimeSpan.FromDays(2)).Date || x.Date.Date == DateTime.Now.Add(TimeSpan.FromDays(1)).Date))
            {
                foreach(AppointmentRequest request in date.Teilnahmen.Where(x => x.State == AppointmentRequestState.NotYetVoted))
                {
                    if(GroupedNotifications.Where(x => x.First == request.Owner.UserName).Count() == 0)
                    {
                        GroupedNotifications.Add(new MutableTuple<string, string>(request.Owner.UserName, $"Freundliche Errinnerung {request.Owner.UserName} du musst im Kalendar {request.EventDate.Event.Name} für den {request.EventDate.Date.ToString("dd.MM.yyyy HH:mm")} abstimmen. {address}/Events/VoteOnCalendar/{calendar.Id}"));
                    }
                    else
                    {
                        GroupedNotifications.Where(x => x.First == request.Owner.UserName).FirstOrDefault().Second += Environment.NewLine + $"Freundliche Errinnerung {request.Owner.UserName} du musst im Kalendar {request.EventDate.Event.Name} für den {request.EventDate.Date.ToString("dd.MM.yyyy HH:mm")} abstimmen. {address}/Events/VoteOnCalendar/{calendar.Id}";
                    }
                }
            }
            foreach(MutableTuple<string,string> tuple in GroupedNotifications)
            {
                
                _eventBus.TriggerEvent(EventType.DiscordWhisperRequested, new DiscordWhisperArgs { UserName = tuple.First , Message = tuple.Second });
            }
        }

        private void RemovePassedEventDates()
        {
            Console.WriteLine("RemovePassedEventDates");
            var EventDatesToRemove = _context.EventDates.Where(x => x.Date < DateTime.Now);
            foreach(EventDate remove in EventDatesToRemove)
            {
                _context.EventDates.Remove(remove);

            }
            _context.SaveChanges();
            foreach(Event calendar in _context.Events)
            {
                var EventDatesInCalendarToRemove = calendar.EventDates.Where(x => x.Date < DateTime.Now);
                foreach (EventDate remove in EventDatesInCalendarToRemove)
                {
                    calendar.EventDates.Remove(remove);
                }
            }
            _context.SaveChanges();
        }
        private void AddEventDatesOnCalendar(Event calendar)
        {
            Console.WriteLine("AddEventDatesOnCalendar");
            // Next 2 weeks
            for (int week = 0; week < 2;week++ )
            {
                foreach(EventDateTemplate template in calendar.EventDateTemplates)
                {
                    EventDate eventdate = template.CreateEventDate(week);
                    eventdate.EventDateTemplateID = template.ID;
                    if(calendar.EventDates.Where( x => x.Date == eventdate.Date).Count() == 0)
                    {
                        eventdate.Teilnahmen = calendar.GenerateAppointmentRequests(eventdate);
                        _context.EventDates.Add(eventdate);
                        calendar.EventDates.Add(eventdate);
                    }
                }
            }
            _context.SaveChanges();
        }
        private void UpdateEventDates(Event calendar)
        {
            foreach (EventDate update in calendar.EventDates)
            {
                var template = _context.EventDateTemplates.Where(x => x.ID == update.EventDateTemplateID).FirstOrDefault();
                if(template != null)
                {
                    update.StartTime = template.StartTime;
                    update.StopTime = template.StopTime;
                    update.Date =  DateTime.ParseExact(update.Date.ToString("dd-MM-yyyy") + " " + update.StartTime.ToString("HH:mm"), "dd-MM-yyyy HH:mm",
                                    System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            _context.SaveChanges();
        }
    }
}