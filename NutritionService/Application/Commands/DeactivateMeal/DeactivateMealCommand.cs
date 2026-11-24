using MediatR;

namespace NutritionService.Application.Commands.DeactivateMeal;

public record DeactivateMealCommand(Guid Id) : IRequest<bool>;
