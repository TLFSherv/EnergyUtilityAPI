# EnergyUtilityAPI
ASP.NET RESTful API to calculate hyper-local household energy expenditures by processing location-specific tariff data and consumption telemetry

This MVC API has two endpoints - one for fetching postcode level electricity meter data for over 1 million UK postcodes, and the second calculates the yearly consumption and cost of electricity based on postcode and household features passed as query string parameters. The API uses FluidValidation for input validation. The API uses a PostgreSQL database.

## Data
The data used by the API comes from many sources. The electricity meter data comes from the Department of Energy Security and Net Zero (DESNZ) Postcode level electricity statistics 2024. The National Energy Efficiency Data-Framework (NEED) containing 4 million records of household energy statistics was used to calculate household feature multipliers (floor area, property type, house age) per NEED region. Using the query string values the API applies the correct multiplier to the median region electricity consumption to estimate actual household consumption.

For example, if the NEED data shows:
- National Median: 2,700 kWh
- Median for "Detached Houses": 3,510 kWh
- Weight (W_type): 3,510/2,700 = 1.30

The NEED data includes electricity consumption values for other factors like council tax band and index of deprivation, but I decided to ignore these factors for the timebeing. 

The final datasource used to construct the dataset utilised by this API was the Ofgem Price Cap data. This data was used to get the unit rate and standing charge for households. There are variations in electricity costs in the UK based on region - the UK is split into 14 distribution network operator (DNO) regions. Therefore it was necessary to map each postcode in the DESNZ dataset to the correct DNO region in order to estimate the correct electricity cost. The most accurate way to link over a million postcodes to a DNO region was to use a DNO licence area boundary JSON file (NESO DNO boundary GeoJSON) and a postcode location dataset (OS Open Code-Point) - and using geopandas in python perform a spatial join to match postcode and dno region based on spatial relationships between geometries. 

## Query string parameter values

