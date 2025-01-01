using Banking.Account.Query.Application.Contracts.Persistence;
using Banking.Account.Query.Application.Models;
using Banking.Account.Query.Domain;
using Banking.Cqrs.Core.Events;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Banking.Account.Query.Infrastructure.Consumers
{
    public class BankAccountConsumerService : IHostedService
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private KafkaSettings _kafkaSettings { get; }

        public BankAccountConsumerService(IServiceScopeFactory factory)
        {
            _bankAccountRepository = factory.CreateScope().ServiceProvider.GetRequiredService<IBankAccountRepository>();
            _kafkaSettings = (factory.CreateScope().ServiceProvider.GetRequiredService<IOptions<KafkaSettings>>()).Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig()
            {
                GroupId = _kafkaSettings.GroupId,
                BootstrapServers = $"{_kafkaSettings.Hostname}:{_kafkaSettings.Port}",
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };

            try
            {
                using (var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build())
                {
                    var bankTopics = new string[]
                    {
                        typeof(AccountOpenedEvent).Name,
                        typeof(AccountClosedEvent).Name,
                        typeof(FundsDepositedEvent).Name,
                        typeof(FundsWithdrawnEvent).Name,
                    };

                    consumerBuilder.Subscribe(bankTopics);
                    var cancellationTokenSource = new CancellationTokenSource();

                    try
                    {
                        while (true)
                        {
                            var consumer = consumerBuilder.Consume(cancellationTokenSource.Token);
                            if (consumer.Topic == typeof(AccountOpenedEvent).Name)
                            {
                                var accountOpenedEvent = JsonConvert.DeserializeObject<AccountOpenedEvent>(consumer.Message.Value)!;
                                var bankAccount = new BankAccount
                                {
                                    Identifier = accountOpenedEvent.Id,
                                    AccountHolder = accountOpenedEvent.AccountHolder,
                                    AccountType = accountOpenedEvent.AccountType,
                                    Balance = accountOpenedEvent.OpeningBalance,
                                    CreationDate = accountOpenedEvent.CreatedDate,
                                };

                                _bankAccountRepository.AddAsync(bankAccount).Wait();
                            }

                            if (consumer.Topic == typeof(AccountClosedEvent).Name)
                            {
                                var accountCloseEvent = JsonConvert.DeserializeObject<AccountClosedEvent>(consumer.Message.Value)!;
                                _bankAccountRepository.DeleteByIdentifier(accountCloseEvent.Id).Wait();
                            }

                            if (consumer.Topic == typeof(FundsDepositedEvent).Name)
                            {
                                var fundsDepositedEvent = JsonConvert.DeserializeObject<FundsDepositedEvent>(consumer.Message.Value)!;
                                var bankAccount = new BankAccount
                                {
                                    Identifier = fundsDepositedEvent.Id,
                                    Balance = fundsDepositedEvent.Amount,
                                };

                                _bankAccountRepository.DepositBankAccountByIdentifier(bankAccount).Wait();
                            }

                            if (consumer.Topic == typeof(FundsWithdrawnEvent).Name)
                            {
                                var fundsWithdrawnEvent = JsonConvert.DeserializeObject<FundsWithdrawnEvent>(consumer.Message.Value)!;
                                var bankAccount = new BankAccount
                                {
                                    Identifier = fundsWithdrawnEvent.Id,
                                    Balance = fundsWithdrawnEvent.Amount,
                                };
                                _bankAccountRepository.WithdrawnBankAccountByIdentifier(bankAccount).Wait();
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        consumerBuilder.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
