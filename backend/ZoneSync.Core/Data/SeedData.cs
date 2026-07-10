using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ZoneSync.Core.Entities;
using ZoneSync.Core.Entities.AlertsTasks;
using ZoneSync.Core.Entities.CropModule;
using ZoneSync.Core.Entities.CropPlanModule;
using ZoneSync.Core.Entities.FarmZone;
using ZoneSync.Core.Entities.GrowthStageModule;
using ZoneSync.Core.Entities.Identity;
using ZoneSync.Core.Entities.Sensors;
using ZoneSync.Core.Entities.StageRequirementModule;
using ZoneSync.Core.Entities.StageInformationModule;
using ZoneSync.Core.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace ZoneSync.Core.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            await context.Database.MigrateAsync();

            if (await context.UserProfiles.AnyAsync())
                return;

            // Roles
            if (!await roleManager.RoleExistsAsync("Owner"))
                await roleManager.CreateAsync(new ApplicationRole { Name = "Owner" });
            if (!await roleManager.RoleExistsAsync("Engineer"))
                await roleManager.CreateAsync(new ApplicationRole { Name = "Engineer" });
            if (!await roleManager.RoleExistsAsync("Farmer"))
                await roleManager.CreateAsync(new ApplicationRole { Name = "Farmer" });

            // Owner user
            var ownerUser = await userManager.FindByEmailAsync("owner@farm.com");
            if (ownerUser == null)
            {
                ownerUser = new ApplicationUser
                {
                    UserName = "owner@farm.com",
                    Email = "owner@farm.com",
                    PhoneNumber = "01000000001",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(ownerUser, "Owner123!");
                await userManager.AddToRoleAsync(ownerUser, "Owner");
            }

            // Engineer user
            var engineerUser = await userManager.FindByEmailAsync("engineer@farm.com");
            if (engineerUser == null)
            {
                engineerUser = new ApplicationUser
                {
                    UserName = "engineer@farm.com",
                    Email = "engineer@farm.com",
                    PhoneNumber = "01000000002",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(engineerUser, "Engineer123!");
                await userManager.AddToRoleAsync(engineerUser, "Engineer");
            }

            // Farmer user
            var farmerUser = await userManager.FindByEmailAsync("farmer@farm.com");
            if (farmerUser == null)
            {
                farmerUser = new ApplicationUser
                {
                    UserName = "farmer@farm.com",
                    Email = "farmer@farm.com",
                    PhoneNumber = "01000000003",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(farmerUser, "Farmer123!");
                await userManager.AddToRoleAsync(farmerUser, "Farmer");
            }

            // User Profiles
            var ownerProfile = new UserProfile
            {
                AspNetUserId = ownerUser.Id,
                Email = ownerUser.Email!,
                UserFirstName = "Ahmed",
                UserLastName = "ElSayed",
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };
            context.UserProfiles.Add(ownerProfile);

            var engineerProfile = new UserProfile
            {
                AspNetUserId = engineerUser.Id,
                Email = engineerUser.Email!,
                UserFirstName = "Mona",
                UserLastName = "Hassan",
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };
            context.UserProfiles.Add(engineerProfile);

            var farmerProfile = new UserProfile
            {
                AspNetUserId = farmerUser.Id,
                Email = farmerUser.Email!,
                UserFirstName = "Ali",
                UserLastName = "Mohamed",
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };
            context.UserProfiles.Add(farmerProfile);

            await context.SaveChangesAsync();

            // Farm
            var farm = new Farm
            {
                FarmName = "Green Valley Farm",
                FarmLocation = "Alexandria, Egypt",
                SoilType = "Clay Loam",
                TotalArea = 50,
                NoOfZones = 0,
                OwnerUserId = ownerProfile.UserId,
                IsDeleted = false
            };
            context.Farms.Add(farm);
            await context.SaveChangesAsync();

            // Farm Memberships
            context.FarmMemberships.Add(new FarmMembership { FarmId = farm.FarmId, UserId = ownerProfile.UserId, RoleType = Enums.FarmRoleType.Owner, JoinedAt = DateTime.Now });
            context.FarmMemberships.Add(new FarmMembership { FarmId = farm.FarmId, UserId = engineerProfile.UserId, RoleType = Enums.FarmRoleType.Engineer, JoinedAt = DateTime.Now });
            context.FarmMemberships.Add(new FarmMembership { FarmId = farm.FarmId, UserId = farmerProfile.UserId, RoleType = Enums.FarmRoleType.Farmer, JoinedAt = DateTime.Now });
            await context.SaveChangesAsync();

            // Zones
            var zone1 = new Zone
            {
                FarmId = farm.FarmId,
                ZoneName = "Field A - Tomatoes",
                ZoneArea = 20,
                ZoneStatus = Enums.ZoneStatus.Planted,
                CreatedByUserId = ownerProfile.UserId,
                SupervisorId = engineerProfile.UserId,
                IsDeleted = false
            };
            context.Zones.Add(zone1);

            var zone2 = new Zone
            {
                FarmId = farm.FarmId,
                ZoneName = "Field B - Wheat",
                ZoneArea = 30,
                ZoneStatus = Enums.ZoneStatus.Planted,
                CreatedByUserId = ownerProfile.UserId,
                SupervisorId = engineerProfile.UserId,
                IsDeleted = false
            };
            context.Zones.Add(zone2);
            await context.SaveChangesAsync();

            // Zone Users
            context.ZoneUsers.Add(new ZoneUser { ZoneId = zone1.ZoneId, UserId = engineerProfile.UserId });
            context.ZoneUsers.Add(new ZoneUser { ZoneId = zone1.ZoneId, UserId = farmerProfile.UserId });
            context.ZoneUsers.Add(new ZoneUser { ZoneId = zone2.ZoneId, UserId = engineerProfile.UserId });
            context.ZoneUsers.Add(new ZoneUser { ZoneId = zone2.ZoneId, UserId = farmerProfile.UserId });
            await context.SaveChangesAsync();

            // Recalculate farm totals
            var farmEntity = await context.Farms.FindAsync(farm.FarmId);
            if (farmEntity != null)
            {
                farmEntity.NoOfZones = 2;
                farmEntity.TotalArea = 50;
            }

            // Activity logs
            context.EntityActivityLogs.Add(new EntityActivityLog
            {
                UserId = ownerProfile.UserId,
                EntityType = Enums.ActivityEntityType.Zone,
                EntityId = zone1.ZoneId,
                ActionType = Enums.ActivityActionType.Create,
                ActionAt = DateTime.Now
            });
            context.EntityActivityLogs.Add(new EntityActivityLog
            {
                UserId = ownerProfile.UserId,
                EntityType = Enums.ActivityEntityType.Zone,
                EntityId = zone2.ZoneId,
                ActionType = Enums.ActivityActionType.Create,
                ActionAt = DateTime.Now
            });
            await context.SaveChangesAsync();

            // Crop Plans
            var tomatoCrop = await context.Crops.FirstOrDefaultAsync(c => c.CropName == "Tomato");
            var wheatCrop = await context.Crops.FirstOrDefaultAsync(c => c.CropName == "Wheat");

            if (tomatoCrop != null)
            {
                var cropPlan1 = new CropPlan
                {
                    CropId = tomatoCrop.CropId,
                    ZoneId = zone1.ZoneId,
                    PlantingDate = new DateTime(2026, 6, 1),
                    PredictedHarvestTime = new DateTime(2026, 8, 1)
                };
                context.CropPlans.Add(cropPlan1);
                await context.SaveChangesAsync();

                // Stage informations for tomato crop plan
                var tomatoStages = await context.GrowthStages
                    .Where(g => g.CropId == tomatoCrop.CropId)
                    .OrderBy(g => g.StageOrder)
                    .ToListAsync();

                foreach (var stage in tomatoStages)
                {
                    context.StageInformations.Add(new StageInformation
                    {
                        StageId = stage.StageId,
                        CropPlanId = cropPlan1.CropPlanId,
                        StageName = stage.StageName,
                        PredictedStartDate = stage.StageOrder == 1 ? cropPlan1.PlantingDate : cropPlan1.PlantingDate.AddDays(tomatoStages.Where(s => s.StageOrder < stage.StageOrder).Sum(s => s.StageDuration)),
                        PredictedEndDate = stage.StageOrder == 1 ? cropPlan1.PlantingDate.AddDays(stage.StageDuration) : cropPlan1.PlantingDate.AddDays(tomatoStages.Where(s => s.StageOrder <= stage.StageOrder).Sum(s => s.StageDuration)),
                        ActualStartDate = stage.StageOrder == 1 ? cropPlan1.PlantingDate : null,
                        StageStatus = stage.StageOrder == 1 ? "Current" : "NotStarted"
                    });
                }
                await context.SaveChangesAsync();

                // Seed check requirements for tomato
                var tomatoReq = await context.StageRequirements
                    .FirstOrDefaultAsync(r => r.StageId == tomatoStages[0].StageId);

                if (tomatoReq != null)
                {
                    context.CheckRequirements.Add(new CheckRequirement
                    {
                        RequirementId = tomatoReq.ReqId,
                        CropPlanId = cropPlan1.CropPlanId,
                        CheckedValue = 55,
                        LastCheckDate = DateTime.Now.AddDays(-1),
                        NextCheckDate = DateTime.Now,
                        IsSatisfied = true
                    });
                    await context.SaveChangesAsync();
                }
            }

            if (wheatCrop != null)
            {
                var cropPlan2 = new CropPlan
                {
                    CropId = wheatCrop.CropId,
                    ZoneId = zone2.ZoneId,
                    PlantingDate = new DateTime(2026, 6, 15),
                    PredictedHarvestTime = new DateTime(2026, 7, 10)
                };
                context.CropPlans.Add(cropPlan2);
                await context.SaveChangesAsync();

                // Stage informations for wheat crop plan
                var wheatStages = await context.GrowthStages
                    .Where(g => g.CropId == wheatCrop.CropId)
                    .OrderBy(g => g.StageOrder)
                    .ToListAsync();

                foreach (var stage in wheatStages)
                {
                    context.StageInformations.Add(new StageInformation
                    {
                        StageId = stage.StageId,
                        CropPlanId = cropPlan2.CropPlanId,
                        StageName = stage.StageName,
                        PredictedStartDate = stage.StageOrder == 1 ? cropPlan2.PlantingDate : cropPlan2.PlantingDate.AddDays(wheatStages.Where(s => s.StageOrder < stage.StageOrder).Sum(s => s.StageDuration)),
                        PredictedEndDate = cropPlan2.PlantingDate.AddDays(wheatStages.Where(s => s.StageOrder <= stage.StageOrder).Sum(s => s.StageDuration)),
                        ActualStartDate = stage.StageOrder == 1 ? cropPlan2.PlantingDate : null,
                        StageStatus = stage.StageOrder == 1 ? "Current" : "NotStarted"
                    });
                }
                await context.SaveChangesAsync();
            }

            // Sensor Instances
            var sensorModel1 = await context.SensorModels.FirstOrDefaultAsync(sm => sm.Id == 1);
            var sensorModel2 = await context.SensorModels.FirstOrDefaultAsync(sm => sm.Id == 2);

            if (sensorModel1 != null)
            {
                var sensor1 = new SensorInstance
                {
                    SensorModelId = sensorModel1.Id,
                    SerialNumber = "SM-100-001",
                    Status = Enums.SensorStatus.Configured
                };
                context.SensorInstances.Add(sensor1);

                var sensor2 = new SensorInstance
                {
                    SensorModelId = sensorModel1.Id,
                    SerialNumber = "SM-100-002",
                    Status = Enums.SensorStatus.Configured
                };
                context.SensorInstances.Add(sensor2);
                await context.SaveChangesAsync();

                // Zone Configurations
                context.ZoneConfigurations.Add(new ZoneConfiguration
                {
                    ZoneId = zone1.ZoneId,
                    SensorInstanceId = sensor1.Id,
                    ConfiguredByUserId = engineerProfile.UserId,
                    ConfiguredAt = DateTime.Now.AddDays(-15)
                });

                context.ZoneConfigurations.Add(new ZoneConfiguration
                {
                    ZoneId = zone2.ZoneId,
                    SensorInstanceId = sensor2.Id,
                    ConfiguredByUserId = engineerProfile.UserId,
                    ConfiguredAt = DateTime.Now.AddDays(-10)
                });
                await context.SaveChangesAsync();

                // Sensor Readings
                var soilMoistureType = await context.MeasurementTypes.FirstOrDefaultAsync(mt => mt.Id == 1);
                var waterLevelType = await context.MeasurementTypes.FirstOrDefaultAsync(mt => mt.Id == 3);

                if (soilMoistureType != null)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        context.SensorReadings.Add(new SensorReading
                        {
                            SensorInstanceId = sensor1.Id,
                            MeasurementTypeId = soilMoistureType.Id,
                            ReadingValue = 50 + (i * 2),
                            ReadingDate = DateTime.Today.AddDays(-i),
                            ReadingTime = new TimeSpan(8, 0, 0)
                        });
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        context.SensorReadings.Add(new SensorReading
                        {
                            SensorInstanceId = sensor2.Id,
                            MeasurementTypeId = soilMoistureType.Id,
                            ReadingValue = 45 + (i * 3),
                            ReadingDate = DateTime.Today.AddDays(-i),
                            ReadingTime = new TimeSpan(8, 0, 0)
                        });
                    }
                }

                if (waterLevelType != null)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        context.SensorReadings.Add(new SensorReading
                        {
                            SensorInstanceId = sensor1.Id,
                            MeasurementTypeId = waterLevelType.Id,
                            ReadingValue = 40 + (i * 5),
                            ReadingDate = DateTime.Today.AddDays(-i * 2),
                            ReadingTime = new TimeSpan(8, 0, 0)
                        });
                    }
                }
                await context.SaveChangesAsync();
            }

            if (sensorModel2 != null)
            {
                var sensor3 = new SensorInstance
                {
                    SensorModelId = sensorModel2.Id,
                    SerialNumber = "TH-200-001",
                    Status = Enums.SensorStatus.Available
                };
                context.SensorInstances.Add(sensor3);
                await context.SaveChangesAsync();

                var tempType = await context.MeasurementTypes.FirstOrDefaultAsync(mt => mt.Id == 2);
                if (tempType != null)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        context.SensorReadings.Add(new SensorReading
                        {
                            SensorInstanceId = sensor3.Id,
                            MeasurementTypeId = tempType.Id,
                            ReadingValue = 22 + (i % 3),
                            ReadingDate = DateTime.Today.AddDays(-i),
                            ReadingTime = new TimeSpan(12, 0, 0)
                        });
                    }
                    await context.SaveChangesAsync();
                }
            }

            // Seed Alerts
            var alert1 = new Entities.AlertsTasks.Alert
            {
                ZoneId = zone1.ZoneId,
                CropPlanId = await context.CropPlans
                    .Where(cp => cp.ZoneId == zone1.ZoneId)
                    .Select(cp => (int?)cp.CropPlanId)
                    .FirstOrDefaultAsync(),
                RequirementId = await context.StageRequirements
                    .Select(r => (int?)r.ReqId)
                    .FirstOrDefaultAsync(),
                CreatedByUserId = engineerProfile.UserId,
                AlertType = Enums.AlertType.OutOfRangeReading,
                AlertSeverity = Enums.AlertSeverity.Medium,
                AlertStatus = Enums.AlertStatus.UnderReview,
                FiringDate = DateTime.Now.AddDays(-2)
            };
            context.Alerts.Add(alert1);

            var alert2 = new Alert
            {
                ZoneId = zone2.ZoneId,
                CropPlanId = await context.CropPlans
                    .Where(cp => cp.ZoneId == zone2.ZoneId)
                    .Select(cp => (int?)cp.CropPlanId)
                    .FirstOrDefaultAsync(),
                CreatedByUserId = engineerProfile.UserId,
                AlertType = Enums.AlertType.Manual,
                AlertSeverity = Enums.AlertSeverity.Low,
                AlertStatus = Enums.AlertStatus.UnderReview,
                FiringDate = DateTime.Now.AddDays(-1)
            };
            context.Alerts.Add(alert2);
            await context.SaveChangesAsync();

            // Seed Tasks
            var task1 = new TaskItem
            {
                ZoneId = zone1.ZoneId,
                CropPlanId = await context.CropPlans
                    .Where(cp => cp.ZoneId == zone1.ZoneId)
                    .Select(cp => (int?)cp.CropPlanId)
                    .FirstOrDefaultAsync(),
                AlertId = alert1.AlertId,
                CreatedByUserId = engineerProfile.UserId,
                TaskName = "Check soil moisture sensors",
                TaskDesc = "Investigate the out-of-range soil moisture readings in Field A",
                CreatedAt = DateTime.Now.AddDays(-2),
                TaskStatus = Enums.TaskStatus.InProgress,
                DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                TaskPriority = Enums.TaskPriority.High,
                TaskType = Enums.TaskType.BasedOnAlert,
                ActualVerificationAfterHours = 48
            };
            context.Tasks.Add(task1);

            var task2 = new TaskItem
            {
                ZoneId = zone2.ZoneId,
                CreatedByUserId = engineerProfile.UserId,
                TaskName = "Irrigation check",
                TaskDesc = "Verify sprinkler system in Field B",
                CreatedAt = DateTime.Now.AddDays(-1),
                TaskStatus = Enums.TaskStatus.Pending,
                DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
                TaskPriority = Enums.TaskPriority.Medium,
                TaskType = Enums.TaskType.Manual
            };
            context.Tasks.Add(task2);
            await context.SaveChangesAsync();

            // Task assignments
            context.TaskUsers.Add(new TaskUser
            {
                TaskId = task1.TaskId,
                UserId = farmerProfile.UserId,
                AssignedAt = DateTime.Now.AddDays(-2)
            });
            context.TaskUsers.Add(new TaskUser
            {
                TaskId = task2.TaskId,
                UserId = farmerProfile.UserId,
                AssignedAt = DateTime.Now.AddDays(-1)
            });
            await context.SaveChangesAsync();

            // Activity logs for crop plans and tasks
            var cropPlans = await context.CropPlans.ToListAsync();
            foreach (var cp in cropPlans)
            {
                context.EntityActivityLogs.Add(new EntityActivityLog
                {
                    UserId = ownerProfile.UserId,
                    EntityType = Enums.ActivityEntityType.CropPlan,
                    EntityId = cp.CropPlanId,
                    ActionType = Enums.ActivityActionType.Create,
                    ActionAt = DateTime.Now
                });
            }

            context.EntityActivityLogs.Add(new EntityActivityLog
            {
                UserId = engineerProfile.UserId,
                EntityType = Enums.ActivityEntityType.Task,
                EntityId = task1.TaskId,
                ActionType = Enums.ActivityActionType.Create,
                ActionAt = DateTime.Now.AddDays(-2)
            });

            context.EntityActivityLogs.Add(new EntityActivityLog
            {
                UserId = engineerProfile.UserId,
                EntityType = Enums.ActivityEntityType.Task,
                EntityId = task2.TaskId,
                ActionType = Enums.ActivityActionType.Create,
                ActionAt = DateTime.Now.AddDays(-1)
            });
            await context.SaveChangesAsync();
        }
    }
}
