using MediatR;
using Yearly.Domain.Models.WeeklyMenuAgg;

namespace Yearly.Application.Menus.Queries;

public record GetAvailableWeeklyMenusQuery() : IRequest<List<WeeklyMenu>>;