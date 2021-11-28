from collections import Counter

file1 = open('part-r-00000', 'r')
Lines = file1.readlines()

dict = {}
# Iterate over lines and fill dict
for line in Lines:
    line = line.strip('\n')
    key, value = line.split('\t')
    dict[key] = int(value)

counter = Counter(dict)
topTen = counter.most_common(10)

for k, v in topTen:
    print('%s: %i' % (k, v))
