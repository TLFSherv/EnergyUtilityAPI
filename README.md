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
The HouseholdSize parameter is for all postcodes except Scotland postcodes.
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

The below options are for Scotland Postcodes only 
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
