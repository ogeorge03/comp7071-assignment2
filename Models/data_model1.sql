
drop table employee;
drop table org;
drop table person;
drop table st_address;

create table st_address (
	id int identity not null primary key,
	street_name nvarchar(255) not null,
	city nvarchar(255) not null,
	state_prov  nvarchar(255) null,
	country  nvarchar(255) not null,
	postal_code varchar(6) not null
);


create unique index idx_stadd_uniq on st_address (
street_name, city, state_prov, country
);

create unique index idx_stadd_uniq2 on st_address (
postal_code, country
);

insert into st_address (street_name, city, state_prov, country, postal_code)
values ('123 fake st', 'Burnaby', 'BC', 'Canada', '123456');

insert into st_address (street_name, city, state_prov, country, postal_code)
values ('456 fake st', 'Burnaby', 'BC', 'Canada', '111222');

insert into st_address (street_name, city, state_prov, country, postal_code)
values ('555 fake st', 'Burnaby', 'BC', 'Canada', '222333');




create table person (
	id int identity not null primary key,
	first_name nvarchar(255) not null,
	middle_name nvarchar(255)  null,
	last_name  nvarchar(255) not null,
	date_of_birth date  not null check (date_of_birth > '1900-01-01' and date_of_birth < current_timestamp),
	primary_residence_id int foreign key references st_address(id) null
);

create unique index idx_uniq_person on person (first_name, last_name, date_of_birth, primary_residence_id)

insert into person (first_name, last_name, date_of_birth, primary_residence_id )
values ('alex', 'dolotov', '1990-01-01', 1)

insert into person (first_name, last_name, date_of_birth )
values ('joe', 'homeless', '1930-01-01')

insert into person (first_name, last_name, date_of_birth )
values ('bob', 'doe', '1950-01-01')

insert into person (first_name, last_name, date_of_birth )
values ('john', 'appleseed', '1970-01-01')

select * from person

create table org (
	id int identity not null primary key,
	parent_org_id int null foreign key references org(id),
	legal_name nvarchar(255) not null,
	primary_address_id int foreign key references st_address(id) null,
	incorporation_address_id int foreign key references st_address(id) null,
	org_type nvarchar(255) not null check(org_type in ('CORP','CHARITY','NGO','OTHER')), -- corporation, charity, etc. 
	legal_contact_id int foreign key references person(id) null
);

insert into org ( legal_name, primary_address_id, org_type, legal_contact_id )
values( 'Shady Acres Home Living', 2, 'CORP', 1 );

select * from org

drop table employee;
create table employee (
	id  int identity not null primary key,
	supervisor_id int  foreign key references employee(id) null,
	org_id int foreign key references org(id) not null,
	person_id int foreign key references person(id) not null,
	emergency_contact_id int foreign key references person(id) null,
	job_title nvarchar(255) null,
	employment_type varchar(50) null check(employment_type in ('FULL-TIME','PART-TIME','ON CALL')),
	salary_hourly varchar(50) null check(salary_hourly in ('SALARY', 'HOURLY')),
	pay_rate_amount money null,
	pay_rate_frequency varchar(50) null check(pay_rate_frequency in ('MONTHLY', 'BI-MONTHLY', 'WEEKLY', 'BI-WEEKLY')) default('BI-WEEKLY'),
	work_status varchar(50) not null default('ACTIVE') check(work_status in ('ACTIVE', 'TERMINATED', 'ON LEAVE', 'SICK', 'VACATION', 'UNKNOWN')),
	employment_start_date date not null default(cast(current_timestamp as date)),
	employment_termination_date date null 
)

create unique index idx_employee_person_org on employee(person_id, org_id);
-- should only allow one employer org at a time... but need to check employment_start_date and employment_termination_date if employees change jobs


insert into employee (org_id, person_id, emergency_contact_id, job_title, employment_type, salary_hourly, pay_rate_amount  )
values (1, 1, 2, 'CEO', 'FULL-TIME', 'SALARY', 100000)

insert into employee (supervisor_id, org_id, person_id, emergency_contact_id, job_title, employment_type, salary_hourly, pay_rate_amount  )
values (1, 1, 2, 1, 'Nurse', 'FULL-TIME', 'HOURLY', 100)


select * from employee inner join person on employee.person_id = person.id

drop table shift_schedule;


create table shift_schedule (
	employee_id int foreign key references employee(id) not null,
	supervisor_id int foreign key references employee(id) not null,
	start_datetime datetime not null,
	hours_scheduled int not null check(hours_scheduled > 0 and hours_scheduled <= 24),
	hours_completed int not null check(hours_completed >= 0 and hours_completed <= 24) default 0,
	comments nvarchar(max) null
)

create unique index idx_shift_uniq on shift_schedule(employee_id, start_datetime)
go


drop table employee_vacation
create table employee_vacation (
	id int identity not null primary key,
	employee_id int foreign key references employee(id) not null,
	vacation_start_date date not null,
	vacation_end_date date not null,
	supervisor_approved bit null,
	supervisor_id int foreign key references employee(id) not null,
	approved_date date null,
	is_paid_vacation bit not null 
);
go

insert into employee_vacation (employee_id,vacation_start_date,vacation_end_date, supervisor_approved, supervisor_id, is_paid_vacation  )
values (2, '2025-01-05', '2025-01-07', 1, 1, 0)

select * from employee_vacation

create table employee_sick_leave (
	employee_id int foreign key references employee(id) not null,
	sick_day date not null,
	doctors_note nvarchar(100) null
)

create unique index idx_sick_leave on employee_sick_leave(employee_id, sick_day);

go

-- add trigger to prevent shift overlap scheduling
--drop trigger check_shift_overlap;
create trigger check_shift_overlap on shift_schedule
	instead of insert as 

	if exists  (	
		select 
			*
		from shift_schedule as x
		inner join inserted as y	
			on x.employee_id = y.employee_id
			and x.start_datetime < dateadd(hour, y.hours_scheduled, y.start_datetime)
			and x.start_datetime >= y.start_datetime
		
	)  throw 51000, 'Cannot create new shift because of existing overlap for employee', 1 ;

	if exists  (	
		select 
			*
		from shift_schedule as x
		inner join inserted as y	
			on x.employee_id = y.employee_id
			and y.start_datetime < dateadd(hour, x.hours_scheduled, x.start_datetime)
			and y.start_datetime >= x.start_datetime
		
	)  throw 51000, 'Cannot create new shift because of existing overlap for employee', 1 ;

	if exists (
		select 
			*
		from employee_vacation as x
		inner join inserted as y	
			on x.employee_id = y.employee_id
			and x.vacation_start_date < dateadd(hour, y.hours_scheduled, y.start_datetime)
			and x.vacation_end_date >= dateadd(hour, y.hours_scheduled, y.start_datetime)
	) throw 51000, 'Cannot create new shift because employee has vacation time scheduled', 1 ;

	if exists (
		select 
			*
		from employee_vacation as x
		inner join inserted as y	
			on x.employee_id = y.employee_id
			and x.vacation_start_date >= y.start_datetime
			and x.vacation_end_date <= y.start_datetime
	) throw 51000, 'Cannot create new shift because employee has vacation time scheduled', 1 ;


	insert into shift_schedule(employee_id, supervisor_id, start_datetime, hours_scheduled, hours_completed, comments)
	select employee_id, supervisor_id, start_datetime, hours_scheduled, hours_completed, comments
	from inserted			
			
	
go

truncate table shift_schedule;

insert into shift_schedule(employee_id,supervisor_id, start_datetime, hours_scheduled, hours_completed)
values(2,1, '2025-01-01 12:00:00', 4, 4)

insert into shift_schedule(employee_id,supervisor_id, start_datetime, hours_scheduled, hours_completed)
values(2,1, '2025-01-02 12:00:00', 4, 2)

insert into shift_schedule(employee_id,supervisor_id, start_datetime, hours_scheduled, hours_completed)
values(2,1, '2025-01-03 12:00:00', 4, 0)

insert into shift_schedule(employee_id,supervisor_id, start_datetime, hours_scheduled, hours_completed)
values(2,1, '2025-01-06 13:00:00', 4, 0)

select * from shift_schedule



create table stat_holidays (
	date date not null primary key,
	holiday varchar(50)
	)

insert into stat_holidays (date, holiday)
values ('2025-01-01', 'New Year')


select 
	*, 
	case 
		when salary_hourly = 'HOURLY' 
			then pay_rate_amount * hours_completed else null end 
		* case when stat_holidays.date is not null then 1.5 else 1 end
			as wages_earned

	from  shift_schedule
	inner join employee on shift_schedule.employee_id = employee.id
	inner join person on employee.person_id = person.id
	left join stat_holidays on 
		( cast(shift_schedule.start_datetime as date) = stat_holidays.date 
		OR 
		 cast(dateadd(hour, hours_scheduled, shift_schedule.start_datetime) as date) = stat_holidays.date 
		)



drop table payroll;
create table payroll (
	id int identity not null primary key,
	employee_id int foreign key references employee(id) not null,
	processed_datetime datetime null,
	regular_pay money not null,
	overtime_pay money not null,
	vacation_pay  money not null,
	tax_withheld money not null,
	payroll_adjustment money null,
	adjustment_reason nvarchar(max) null,

	pay_period_start_date date not null,
	pay_period_end_date date not null,

	direct_deposit_number varchar(100) null,
	check_number  varchar(100) null
	
)

insert into payroll(employee_id, regular_pay, overtime_pay, vacation_pay, tax_withheld, pay_period_start_date, pay_period_end_date )
values (1, 100000, 0, 0, 50000, '2024-12-01', '2024-12-31' )

insert into payroll(employee_id, regular_pay, overtime_pay, vacation_pay, tax_withheld, pay_period_start_date, pay_period_end_date )
values (2, 200, 600, 0, 200, '2025-01-01', '2024-01-05' )

select * from payroll


create table certification (
	id int identity not null primary key,
	certification_description  nvarchar(200) not null,
	certification_authority  nvarchar(200),
	certification_number  nvarchar(200)
)

insert into certification (certification_description, certification_authority)
values ('physio', 'BC Physio Association')

insert into certification (certification_description, certification_authority)
values ('massage', 'BC Massage Association')

select * from certification

create table employee_certifications (
	employee_id int foreign key references employee(id) not null,
	certification_id int foreign key references certification(id) not null,
	certification_valid_until date null,
	certification_received_date date null,
	constraint pk_emp_cert primary key(employee_id, certification_id)
)

insert into employee_certifications (employee_id, certification_id)
values (1, 1), (2, 2);


create table customer_service (
	id int identity not null primary key,
	service_description nvarchar(200),
	hourly_rate money null,
	certification_required int foreign key references certification(id) null
)

truncate table customer_service;
insert into customer_service ( service_description, hourly_rate, certification_required)
values('house keeping', 100, null), ('full body massage', 200, 2), ('physio', 200, 1), ('bingo', 10, null)


create table contact_information(
	contact_id int identity not null primary key,
	person_id int foreign key references person(id) not null,
	contact_type varchar(50) not null check (contact_type in ('phone', 'email', 'other')),
	contact_info nvarchar(200) not null
)

create unique index idx_contact_information_uniq on contact_information(person_id, contact_info);
insert into contact_information (person_id, contact_type, contact_info)
values(5, 'phone', '604 555 1122')

drop table customer;
create table customer (
	id int identity not null primary key,
	person_id int foreign key references person(id) not null unique,
	customer_notes nvarchar(max),
	payment_information nvarchar(500),
	primary_contact_id int foreign key references contact_information(contact_id) null
)


select * from person


select * from contact_information

insert into customer(person_id)
values(5)

select * from customer;

create table fascility (
	id int identity not null primary key,
	st_address_id int foreign key references st_address(id),
	name nvarchar(100) not null,
	room varchar(50) null,
	description nvarchar(250) null,
	room_rate money null
)

drop table customer_service_scheduled;
create table customer_service_scheduled (
	id int identity not null primary key,
	customer_id int foreign key references customer(id) not null,
	customer_service_id int  foreign key references customer_service(id) not null,
	fascility_id  int  foreign key references fascility(id) null,
	scheduled_datetime datetime not null,
	rendered_datetime datetime null,
	service_fee money null,
	customer_satisfaction_rating int null check(customer_satisfaction_rating is null or (customer_satisfaction_rating >= 0 and customer_satisfaction_rating <= 10)),
	comments nvarchar(max)
)

drop table employee_service_assignment;
create table employee_service_assignment(
	customer_service_scheduled_id  int  foreign key references customer_service_scheduled(id) not null,
	employee_id int  foreign key references employee(id) not null,
	employee_service_rate money null,
	constraint pk_employee_service_assignment primary key (customer_service_scheduled_id, employee_id)
)


create table customer_invoice(
	id int identity not null primary key,
	service_rendered_id  int  foreign key references customer_service_scheduled(id),
	service_fee money,
	employee_service_rate money,
	fascility_rate money,
	tax money,
	payment_due_date date,
	invoice_date date
)

create table asset_type (
	id int identity not null primary key,
	parent_asset_type_id int foreign key references asset_type(id) null,
	name nvarchar(200) not null,
	description nvarchar(200) null
)

insert into asset_type(parent_asset_type_id, name, description)
values (null, 'sedan', 'vehicle'), (null, 'suit', null), (null, 'land', null)

select * from asset_type
insert into asset_type(parent_asset_type_id, name, description)
values(3, 'building', null)

insert into asset_type(parent_asset_type_id, name, description)
values(4, 'room', null)

select * from asset_type
select * from st_address;

drop table asset;
create table asset (
	id int identity not null primary key,
	parend_asset_id int foreign key references asset(id) null,
	asset_type_id int  foreign key references asset_type(id) not null,
	name nvarchar(200) not null,
	color varchar(100) null,
	rental_rate money,
	lease_rate money,
	book_value money,
	depriciation money, 
	st_address_id int  foreign key references st_address(id) null,
	description nvarchar(max),
	age_years int null
)

create unique index idx_asset_uniq_name on asset(parend_asset_id, asset_type_id, name);
truncate table asset;

insert into asset (asset_type_id, name, color, book_value, depriciation, st_address_id, age_years)
values (4, 'appartment building', 'white', 20000000, 2000000, 3, 10)

insert into asset (parend_asset_id, asset_type_id, name, rental_rate)
values (1, 5, '#101', 2000), (1, 5, '#102', 2000), (1, 5, '#103', 2000), (1, 5, '#104', 2000)

select * from asset 
	inner join asset_type on asset.asset_type_id = asset_type.id
	left join st_address on asset.st_address_id = st_address.id

drop table renter;
create table renter(
	id int identity not null primary key,
	person_id int foreign key references person(id) not null,
	passed_credit_check bit null,
	emergency_contact_id  int foreign key references person(id)  null,
	family_doctor_id  int foreign key references person(id)  null
)

insert into renter(person_id, passed_credit_check, emergency_contact_id)
values(6, 1, 5)

drop table renter_asset_agreement;
create table renter_asset_agreement(
	id int identity not null primary key,
	renter_id  int foreign key references renter(id) not null,
	rental_asset_id  int foreign key references asset(id) not null,
	contract_rental_rate money, 
	rental_start_date date not null, 
	rental_type varchar(50) not null check(rental_type in ('MONTHLY', 'PREPAID', 'ANNUAL')) default('MONTHLY'),
	agreement_expiration_date date null,
	notes nvarchar(600) null
)

create unique index idx_renter_asset_agreement on renter_asset_agreement(renter_id, rental_asset_id, rental_start_date)

select * from renter
select * from asset

insert into renter_asset_agreement(renter_id, rental_asset_id, contract_rental_rate, rental_start_date)
values(1, 2, 1000, '2022-01-01'), (1, 2, 1500, '2023-01-01'), (1, 2, 2000, '2024-01-01')

select * from renter_asset

create table rental_invoice (
	id int identity not null primary key,
	renter_id  int foreign key references renter(id) not null,
	asset_id  int foreign key references asset(id) not null,
	renter_asset_agreement_id   int foreign key references renter_asset_agreement(id) not null,
	rental_fee money,
	tax money, 
	adjustments money,
	adjustments_reason nvarchar(200) null,
	invoice_date date, 
	payment_due_date date
)



















































