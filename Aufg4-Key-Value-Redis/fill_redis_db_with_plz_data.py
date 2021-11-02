import pandas as pd
import redis

# Connect to db:
CONNECTION_POOL = redis.ConnectionPool(host='localhost',
                                       port=6379,
                                       db=0)

redis_db = redis.Redis(connection_pool=CONNECTION_POOL)

# Read file into pandas Dataframe:
json_pandas_dataframe = pd.read_json('plz.data', lines=True)

# print(json_data)
for index, row in json_pandas_dataframe.iterrows():
    # print(row['_id'], row['city'], row['loc'], row['pop'], row['state'])
    zip_code_row = row['_id']
    city = row['city']
    key_value_pairs_row = {'city': city,
                           'loc': ",".join(format(elem, ".3f") for elem in row['loc']),
                           'pop': str(row['pop']),
                           'state': row['state']}
    redis_db.hmset(zip_code_row, key_value_pairs_row)  # Exercise 4b (retrieve city and state for zip code)
    redis_db.sadd(city, str(zip_code_row))  # Exercise 4c (retrieve zip code for given city)
#
# Disconnect from Server
CONNECTION_POOL.disconnect()
