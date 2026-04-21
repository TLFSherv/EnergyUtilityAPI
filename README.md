# EnergyUtilityAPI
ASP.NET RESTful API to calculate hyper-local household energy expenditures by processing location-specific tariff data and consumption telemetry. 

## API Endpoints
This MVC API has two endpoints - one for fetching postcode level electricity meter data, and the second calculates the yearly consumption and cost of electricity based on postcode and household features passed as query string parameters. 
1. api/energy-utility?Postcode=[postcode]
2. api/energy-utility/cost?Postcode=[postcode]&PaymentMethodId=[PaymentMethodOpt]&MeterTypeId=[MeterTypeOpt]

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

## Query string parameter values
The query string parameters in the first table except HouseholdSize can be used for all valid UK postcodes including Scotland. The HouseholdSize parameter cannot be used for Scotland postcodes. The equivalent HouseholdSize parameter for Scotland postcodes can be found in the bottom table, as well as other Scotland postcode specific parameters.
### Table 1 (All UK Postcodes except HouseholdSize)
| Parameter       | Option | Description|      
|-----------------|--------|------------|      
|PropertyType     |1 |Detached |	          
|PropertyType     |2 |Semi detached |       
|PropertyType     |3 |Mid terrace |	  
|PropertyType     |4 |End terrace |	  
|PropertyType     |5 |Bungalow |	  
|PropertyType     |6 |Flat |	      
|PropertyAge      |1 |before 1930 |	  
|PropertyAge      |2 |1930-1972 |	  
|PropertyAge      |3 |1973-1999 |	  
|PropertyAge      |4 |2000 or later | 
|FloorArea        |1 |50 or less |	  
|FloorArea        |2 |51 - 100 |	  
|FloorArea        |3 |101 - 150 |	  
|FloorArea        |4 |151 - 200 |	  
|FloorArea        |5 |over 200 |	  
|HouseholdSize    |1 |1-2 People |	  
|HouseholdSize    |2 |2-3 People |	  
|HouseholdSize    |3 |4+ People |	

### Table 2 (Scotland postcodes only)
| Parameter       | Option| Description|
|-----------------|-------|------------|
|PropertyType     |7 |Terraced |	  
|NumberOfBedrooms |1 |1 |	          
|NumberOfBedrooms |2 |2 |	          
|NumberOfBedrooms |3 |3 |	          
|NumberOfBedrooms |4 |4 |	          
|NumberOfBedrooms |5 |5 or more |	  
|NumberOfAdults   |1 |1 |	          
|NumberOfAdults   |2 |2 |	          
|NumberOfAdults   |3 |3 |	         
|NumberOfAdults   |4 |4 |	          
|NumberOfAdults   |5 |5 or more |	  
|PropertyAge     |12|2010-onwards |  
|PropertyAge      |11|2000-2009 |	  
|PropertyAge      |10|1980-1999 |	 
|PropertyAge      |9 |1955-1979 |	 
|PropertyAge      |8 |1946-1954 |	  
|PropertyAge      |7 |1920-1945 |	  
|PropertyAge      |6 |1871-1919 |	  
|PropertyAge      |5 |Up to 1870 |	
