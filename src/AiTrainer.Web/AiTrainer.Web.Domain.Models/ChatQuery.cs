namespace AiTrainer.Web.Domain.Models;

public abstract class ChatQuery<TEquatable>: DomainModel<TEquatable> where TEquatable : ChatQuery<TEquatable> 
{}