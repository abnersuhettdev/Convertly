# 24 — Pagamento Real Futuro

Pagamento real não entra no MVP.

## Futuro

Possíveis gateways:

- Stripe;
- Mercado Pago;
- Paddle.

## Fluxo futuro

1. usuário escolhe plano;
2. backend cria checkout;
3. gateway confirma pagamento;
4. webhook atualiza assinatura;
5. usuário recebe limite do plano.

## Campos futuros

```txt
provider
provider_customer_id
provider_subscription_id
current_period_start
current_period_end
cancel_at_period_end
```

## Como apresentar

No README:

> O MVP usa upgrade simulado para demonstrar o modelo SaaS. A arquitetura de planos e assinaturas foi criada para facilitar uma integração futura com gateway de pagamento.
