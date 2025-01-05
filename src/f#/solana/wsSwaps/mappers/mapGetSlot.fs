module mapGetSlot

open dbMigration.models

open responseGetSlots

let mapToEntity (responseGetSlotsDTO: responseGetSlots) = 
    let res = new slots()

    res.numberInt <- responseGetSlotsDTO.result

    res            
