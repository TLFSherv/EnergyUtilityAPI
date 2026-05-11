# EnergyUtilityAPI
This project is a high-performance RESTful API designed to calculate hyper-local household energy expenditures by processing location-specific tariff data and consumption telemetry. Built using ASP.NET Core, the system ingests raw utility data and applies deterministic power models to provide precise financial projections and usage forecasts. I architected the backend to handle complex relational data mapping with Entity Framework Core, ensuring that high-density datasets—such as regional energy grid fluctuations and varying tariff structures—are processed with enterprise-grade reliability and data integrity.

To ensure the API is "client-ready" for modern frontends, I focused on building a highly interoperable architecture that delivers sub-second response times for data-heavy queries. The service is engineered to integrate seamlessly with analytical dashboards (such as those built in Next.js and D3.js), transforming raw backend telemetry into actionable insights for users. This project serves as a deep dive into C# design patterns, asynchronous programming, and the optimization of relational databases for high-stakes environmental and financial modeling.

A demo version of this API is currently live on Render. You can create an account and get an API key to use the API in your own service, access the datasets, or learn more about the API at the link shown below. Please be patient when first accessing the app/api as it must spin-up after in-activity.

## Live demo
https://energyutilityapp.onrender.com

## API Endpoints
This MVC API has a single endpoint for fetching postcode level electricity meter data, and calculating the yearly consumption and cost of electricity based on postcode and household features passed as query string parameters. Learn more by using the link above.

## Tech Stack
![Static Badge](https://img.shields.io/badge/ASP.NET_Core-black?logo=ASP.NET)
![Static Badge](https://img.shields.io/badge/C%23-purple?logo=C%23)
![Static Badge](https://img.shields.io/badge/PostgreSQL-white?logo=postgresql)

## Data
The data used by the API comes from many sources. The electricity meter data comes from the Department of Energy Security and Net Zero (DESNZ) Postcode level electricity statistics 2024. The National Energy Efficiency Data-Framework (NEED) was used to calculate household feature multipliers (floor area, property type, house age) per NEED region. The NEED dataset also included electricity consumption values for factors like council tax band and index of deprivation, maybe I'll include these features in a future version of the Energy Utility API. Using the statistics in the NEED dataset the API applies the correct multiplier to the median region electricity consumption to estimate actual household consumption:

For example, if the NEED data shows:
- National Median: 2,700 kWh
- Median for "Detached Houses": 3,510 kWh
- Weight (W_type): 3,510/2,700 = 1.30

The Ofgem Price Cap dataset was used to get the unit rate and standing charge for households. There are variations in electricity costs in the UK based on region (the UK is split into 14 distribution network operator (DNO) regions) so it was necessary to map each postcode in the DESNZ dataset to the correct DNO region. The most accurate way to link over a million postcodes to a DNO region was to use a DNO licence area boundary JSON file (NESO DNO boundary GeoJSON) and a postcode location dataset (OS Open Code-Point) - and using geopandas in python perform a spatial join to match postcode and dno region based on spatial relationships between geometries. 
