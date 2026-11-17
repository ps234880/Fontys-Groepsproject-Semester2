insert into role (id, name) values (1, 'Sales'), (2, 'Development');

insert into status (id, name) values (1, 'New'), (2, 'In progress'), (3, 'Won'), (4, 'Lost');

-- Demo password for every employee below: sales2dev-demo
insert into employee (id, role_id, firstName, lastName, email, phoneNumber, password, createdAt)
values (1, 1, 'Milan', 'Vermeer', 'milan@example.com', '0612345678', 'PBKDF2-SHA256$100000$2x4HhLVqg0auPlrDzV4M3w==$EhunfQY505/JTw6cMfz/54YETpZ7QHgLjdJt4wOgAcs=', now()),
       (2, 1, 'Lotte', 'Bakker', 'lotte@example.com', '0612345679', 'PBKDF2-SHA256$100000$mcNO24v2IHR7GvpRyMxmyw==$zr+xje2S4lBiSNd7UOtO+164g94YbCenESHIvIDxb8o=', now()),
       (3, 2, 'Sanne', 'de Groot', 'sanne@example.com', '0612345680', 'PBKDF2-SHA256$100000$CVcQaeyhBPXkBQ2AL47LPQ==$3QJqK+wdutforbdaGE3XJ6pTLIUACHGDWr4dMusVA94=', now());

insert into module (name, description, priceInCents, productionTimeInWeeks, createdAt)
values ('auth-gateway', 'OAuth2/OIDC gateway with session management and MFA', 1250000, 4, now()),
       ('case-tracker', 'Case and status management with configurable stages', 980000, 3, now()),
       ('notification-hub', 'Email, SMS and push from one template manager', 640000, 2, now());

insert into client (id, companyName, firstName, lastName, email, phoneNumber, createdAt)
values (1, 'Gemeente Tilburg', 'Anouk', 'Jansen', 'anouk@tilburg.example', '0131234567', now()),
       (2, 'Sportbond NL', 'Daan', 'de Wit', 'daan@sportbond.example', '0307654321', now());

insert into deal (client_id, employee_id, status_id, title, requirement, estimatedDate, createdAt)
values (1, 1, 2, 'Citizen Portal 2027', 'Online portal where residents can book appointments and follow their applications.', '2026-11-15', now()),
       (2, 2, 1, 'Member portal', 'Portal for members with login and event registration.', '2026-12-20', now());

insert into deal_module (deal_id, module_id) values (1, 1), (1, 3);

insert into comment (employee_id, deal_id, message, createdAt)
values (3, 1, 'Login can reuse auth-gateway. Booking appointments needs a custom calendar module, about three weeks of work.', now()),
       (1, 1, 'Thanks. The customer is fine with a redirect for the login. I will ask about the calendar.', now());
