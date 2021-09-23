SELECT s.Period, s.IdCC, s.IdER, SUM(s.dLimitCost) as dLimitCost, SUM(s.dNormCost) as dNormCost, SUM(s.dSumCost) as dSumCost
FROM
(SELECT  c.Period as Period,  c.IdCC as IdCC, c.IdER as IdER, c.IsNorm as IsNorm, 
ROUND(SUM(CASE WHEN c.IsNorm = 0 THEN c.FactCost - p.FactCost ELSE 0 END),0) as dLimitCost,
ROUND(SUM(CASE WHEN c.IsNorm = 1 THEN c.FactCost - p.FactCost / p.Produced * c.Produced ELSE 0 END),0) as dNormCost,
ROUND((SUM(CASE WHEN c.IsNorm = 0 THEN c.FactCost - p.FactCost ELSE 0 END) +SUM(CASE WHEN c.IsNorm = 1 THEN c.FactCost - p.FactCost / p.Produced * c.Produced ELSE 0 END)), 0) as dSumCost
FROM 
(SELECT 
	Period, (Period / 100) as Year, (Period - ((Period / 100) * 100)) as Month, 
	IdCC, CCName, IdER, ERName, ERShortName, IdProduct, 
	SUM(FactCost) as FactCost, SUM(Produced) as Produced, IsNorm
FROM UseAllCosts 
WHERE 
	NOT(IdCC == 56 AND IdER == 966) 
	AND Period IN ( 202001, 202002, 202003, 202004, 2020,05, 202006, 202007, 202008, 202009, 202010, 202011, 202012 ) 
	AND IdCC IN ( 16, 23, 56, 70, 71, 110, 501 ) 
	AND IdER IN ( 951, 955, 966, 990, 28462 ) 
GROUP BY Period, IdCC, IdER, IdProduct) c
JOIN 
(SELECT 
	Period, (Period / 100) as Year, (Period - ((Period / 100) * 100)) as Month, 
	IdCC, CCName, IdER, ERName, ERShortName, IdProduct, 
	SUM(FactCost) as FactCost, SUM(Produced) as Produced, IsNorm 
FROM UseAllCosts 
WHERE 
	NOT(IdCC == 56 AND IdER == 966) 
	AND Period IN ( 201901, 201902, 201903, 201904, 2019,05, 201906, 201907, 201908, 201909, 201910, 201911, 201912 ) 
	AND IdCC IN ( 16, 23, 56, 70, 71, 110, 501 ) 
	AND IdER IN ( 951, 955, 966, 990, 28462 ) 
GROUP BY Period, IdCC, IdER, IdProduct) p
ON c.Year = p.Year + 1 AND c.Month = p.Month AND c.IdCC = p.IdCC AND c.IdER = p.IdER AND c.IdProduct = p.IdProduct
GROUP BY c.Period, c.IdCC, c.IdER) s
GROUP BY s.IdCC