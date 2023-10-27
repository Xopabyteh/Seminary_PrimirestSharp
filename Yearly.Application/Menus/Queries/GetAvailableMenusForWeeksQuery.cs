using MediatR;
using Yearly.Domain.Models.MenuForWeekAgg;

namespace Yearly.Application.Menus.Queries;

public record GetAvailableMenusForWeeksQuery() : IRequest<List<MenuForWeek>>;