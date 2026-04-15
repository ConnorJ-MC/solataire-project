using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
/*
 * This file is used to set assembly-level attributes for the test project.
 * In this case, we are disabling test parallelization to ensure that tests
 * that rely on shared state (like the GameManager singleton) do not interfere
 * with each other when run in parallel.
 *
 * If you have tests that can safely run in parallel, you can remove this attribute
 * or set it to true. However, for this particular test suite, we want to ensure
 * that tests run sequentially to avoid issues with shared state.
 */