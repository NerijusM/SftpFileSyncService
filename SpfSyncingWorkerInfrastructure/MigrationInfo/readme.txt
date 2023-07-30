 Pradėti migraciją:

 Inicijavimui paleisti:
 1. Atsidaryti Package Manager Console
 2. Paleisti: Add-Migration InitialMigration -StartupProject SpfSyncingService -Project SpfSyncingWorkerInfrastructure

 
 Duomenu bazes pakeitimams:
 1. Atsidaryti Package Manager Console
 2. Paleisti: Update-Database -StartupProject SpfSyncingService -Project SpfSyncingWorkerInfrastructure