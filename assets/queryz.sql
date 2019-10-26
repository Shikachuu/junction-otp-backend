
@DEPRECATED
SELECT ST_Distance(ST_Transform(ST_LineFromEncodedPolyline('wow`HqbqsB}@u@QQwB_BCLBMhAv@DPFPFPBj@kFnRy@dDEd@wAbFcArDs@~BkAtDY~@m@xBa@pBaAhFCd@yCzLkDbN{EfReD`NeCtJaDrMeBzGe@tBuA|Fg@fB{@pCy@nCeApCs@fBk@zA{F`O]`AkBfF}@~AS@SE[Ec@CoEl@y@LmDvAoF~B_Bn@aAh@kAt@cDtBw@l@]RaCxG}@zCwBhGQh@QTe@TMBq@FQ@MBIBmBEcHIoDDyDIgBC_BEaBAiKOgCHm@Am@Cg@?e@?sCCMIgGYa@Cs@OICsAaA}@m@QOy@m@DOENqBuAyA{@y@e@EAGBcFmDuDiCAMSWYSGE'),23700),ST_Transform(ST_SetSRID(ST_MakePoint(19.074642,47.486211), 4326),23700));


@UPDATED
SELECT * FROM csv WHERE (SELECT ST_Distance(ST_Transform(ST_LineFromEncodedPolyline("+polyline+"),23700),ST_Transform(ST_SetSRID(ST_MakePoint(csv.geo_coord), 4326),23700)) < 1000);

SELECT * FROM csv WHERE (SELECT ST_DWithin(ST_Transform(ST_LineFromEncodedPolyline('wow`HqbqsB}@u@QQwB_BCLBMhAv@DPFPFPBj@kFnRy@dDEd@wAbFcArDs@~BkAtDY~@m@xBa@pBaAhFCd@yCzLkDbN{EfReD`NeCtJaDrMeBzGe@tBuA|Fg@fB{@pCy@nCeApCs@fBk@zA{F`O]`AkBfF}@~AS@SE[Ec@CoEl@y@LmDvAoF~B_Bn@aAh@kAt@cDtBw@l@]RaCxG}@zCwBhGQh@QTe@TMBq@FQ@MBIBmBEcHIoDDyDIgBC_BEaBAiKOgCHm@Am@Cg@?e@?sCCMIgGYa@Cs@OICsAaA}@m@QOy@m@DOENqBuAyA{@y@e@EAGBcFmDuDiCAMSWYSGE'),23700),ST_Transform(csv.geo_coord,23700),1000));
/* Type hibás de működő! */